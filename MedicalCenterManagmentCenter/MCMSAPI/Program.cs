
using MCMSAPI.Authorization;
using MCMSAPI.dtos.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;


namespace MCMSAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = builder.Configuration.GetSection("Jwt");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5500") // exact origin
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // required
        });

            });

            builder.Services.AddRateLimiter(options =>
            {
                // =========================================================
                // 🔐 LOGIN POLICY (Very strict - brute force protection)
                // =========================================================
                options.AddPolicy("LoginPolicy", context =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 4,                     
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        }));

                // =========================================================
                // 🔄 REFRESH TOKEN POLICY (Moderate)
                // =========================================================
                options.AddPolicy("RefreshPolicy", context =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 10,                    // 10 refresh per minute
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        }));

                // =========================================================
                // 📝 REGISTER POLICY (Prevent spam accounts)
                // =========================================================
                options.AddPolicy("RegisterPolicy", context =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 3,                     // 3 registrations per minute
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        }));

                // =========================================================
                // 👤 AUTHENTICATED USER POLICY (System protection)
                // =========================================================
                options.AddPolicy("UserLimiterPolicy", context =>
                {
                    if (!context.User.Identity?.IsAuthenticated ?? true)
                    {
                        return RateLimitPartition.GetNoLimiter("unauthenticated");
                    }

                    var userId = context.User.FindFirst("sub")?.Value;

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: userId!,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 100,                   // 100 requests per minute
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        });
                });

                // =========================================================
                // 🌍 GLOBAL IP PROTECTION (Optional but recommended)
                // Protect entire system from flooding
                // =========================================================
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 400,                   // 300 requests per minute per IP
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 25,
                            QueueLimit = 0
                        }));

                // =========================================================
                // 🚫 Custom 429 Response
                // =========================================================
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsync(
                        """
            {
                "status": 429,
                "message": "Too many requests. Please slow down."
            }
            """,
                        token);
                };

                options.RejectionStatusCode = 429;
            });

            // Add services to the container.
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"]

            };
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("accessToken"))
                    {
                        context.Token = context.Request.Cookies["accessToken"];
                    }
                    return Task.CompletedTask;
                }
            };

        });


            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter your JWT token like: **Bearer your_token_here**"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });


            builder.Services.AddSingleton<IAuthorizationHandler, OwnershipHandler>();
            builder.Services.AddAuthorization();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                 
            }); ;
           

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("OwnerOnly", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
           
            app.UseHttpsRedirection();
           
            
            app.UseStaticFiles();
            

            app.UseRouting();
            app.UseAuthentication();
            app.UseCors("AllowFrontend");
            app.UseAuthorization();
            app.UseRateLimiter();

            app.MapControllers().RequireRateLimiting("UserLimiterPolicy");

            app.Run();
        }
    }
}
