
using MCMSAPI.Authorization;
using MCMSAPI.dtos.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


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
            

            app.MapControllers();

            app.Run();
        }
    }
}
