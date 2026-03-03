using MCMSAPI.dtos;
using MCMSAPI.Helper;
using MCMSBLL;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace MCMSAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserAccountService _userAccountService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITwoFactorCodeData _twoFactorCodeData;
        private readonly IEmailSenderService _emailSender;
        private void SetAccessTokenCookie(string accessToken, int expiresMinutes)
        {
            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                IsEssential = true
            });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(14),
                IsEssential = true
            });
        }

        private void ClearAuthCookies()
        {
            // Delete cookies on common paths to avoid leaving behind older scoped cookies
            // (e.g., ones created before we set Path = "/" explicitly).
            Response.Cookies.Delete("accessToken", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("accessToken", new CookieOptions { Path = "/api/Auth" });
            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/api/Auth" });
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/api/Auth" });
        }

        public AuthController(
            IConfiguration config,
            IUserAccountService userAccountService,
            IRefreshTokenService refreshTokenService,
            ITwoFactorCodeData twoFactorCodeData,
            IEmailSenderService emailSender)
        {
            _config = config;
            _userAccountService = userAccountService;
            _refreshTokenService = refreshTokenService;
            _twoFactorCodeData = twoFactorCodeData;
            _emailSender = emailSender;
        }

        private (string Key, string Issuer, string Audience, int ExpiresInMinutes) GetJwtOptions()
        {
            string key = _config["Jwt:Key"] ?? throw new Exception("JWT Key missing");
            string issuer = _config["Jwt:Issuer"] ?? throw new Exception("JWT Issuer missing");
            string audience = _config["Jwt:Audience"] ?? throw new Exception("JWT Audience missing");
            int expires = int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "15");

            return (key, issuer, audience, expires);
        }

        // ======================================================
        // LOGIN
        // ======================================================
        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userAccountService.FindByEmailAsync(dto.Email);

            if (user == null || !user.IsActive)
                return Unauthorized("Invalid credentials.");

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            if (user.Is2FAEnabled)
            {
                Task.Run(() => _emailSender.SendTwoFactorCodeAsync(user.Email, user.UserId));
                return Ok(new
                {
                    user.UserId,
                    message = "2FA code sent."
                });
            }

            return await IssueTokens(user);
        }

        // ======================================================
        // CONFIRM 2FA
        // ======================================================
        [AllowAnonymous]
        [EnableRateLimiting("RegisterPolicy")]
        [HttpPost("confirm-2fa")]
        public async Task<IActionResult> Confirm2FA(Guid userId, string code)
        {
            var result = await _twoFactorCodeData.GetLatestCodeAsync(userId);
            if (result is null) return BadRequest("Invalid code.");

            var storedCode = result.Value.Code;
            var expiry = result.Value.Expiry;
            var isUsed = result.Value.IsUsed;

            if (isUsed || expiry < DateTime.UtcNow || storedCode != code)
                return BadRequest("Invalid code.");

            await _twoFactorCodeData.MarkAsUsedAsync(userId, code);

            var user = await _userAccountService.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            return await IssueTokens(user);
        }

        // ======================================================
        // REFRESH (ROTATION BASED)
        // ======================================================
        [AllowAnonymous]
        [EnableRateLimiting("RefreshPolicy")]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // ✅ Read refresh token from cookie instead of body
            if (!Request.Cookies.TryGetValue("refreshToken", out var oldRefreshToken))
                return Unauthorized("Refresh token missing.");

            var rotateResult = await _refreshTokenService.RotateTokenAsync(
                oldRefreshToken,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString()
            );

            if (!rotateResult.Success)
            {
                ClearAuthCookies();
                return Unauthorized("Invalid refresh token.");
            }

            if (string.IsNullOrWhiteSpace(rotateResult.NewRefreshToken))
            {
                ClearAuthCookies();
                return Unauthorized("Invalid refresh token.");
            }

            var user = await _userAccountService.FindByIdAsync(rotateResult.UserId);
            if (user == null)
                return Unauthorized("User not found.");

            var jwt = GetJwtOptions();

            var newAccessToken = JwtHelper.GenerateJwtToken(
                user.UserId,
                user.PersonId,
                user.RoleId,
                jwt.Key,
                jwt.Issuer,
                jwt.Audience,
                jwt.ExpiresInMinutes
            );

            // ✅ Reset cookies
            SetAccessTokenCookie(newAccessToken, jwt.ExpiresInMinutes);
            SetRefreshTokenCookie(rotateResult.NewRefreshToken);

            return Ok(new
            {
                message = "Token refreshed",
                accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(jwt.ExpiresInMinutes)
            });
        }

        // ======================================================
        // LOGOUT (single session)
        // ======================================================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (!Request.Cookies.TryGetValue("refreshToken", out var oldRefreshToken))
            {
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return BadRequest("Invalid token claims.");

                await _refreshTokenService.RevokeAllTokensAsync(
                    userId,
                    HttpContext.Connection.RemoteIpAddress?.ToString()
                );

            }
            else 
            {
                await _refreshTokenService.RevokeTokenAsync(
                        oldRefreshToken,
                        HttpContext.Connection.RemoteIpAddress?.ToString()
                    );
            }
                ClearAuthCookies();

            return Ok("Logged out successfully.");
        }

        // ======================================================
        // LOGOUT ALL DEVICES
        // ======================================================
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userIdClaim =
                User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid token claims.");

            await _refreshTokenService.RevokeAllTokensAsync(
                userId,
                HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok("Logged out from all devices.");
        }



        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId =
                User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
            var personId = User.Claims.FirstOrDefault(c => c.Type == "personId")?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            

            return Ok(new
            {
                userId,
                roleId,
                personId,
                role
            });
        }
        // ======================================================
        // PRIVATE HELPER: Issue Tokens
        // ======================================================
        private async Task<IActionResult> IssueTokens(UserAccount user)
        {
            var jwt = GetJwtOptions();

            var accessToken = JwtHelper.GenerateJwtToken(
                user.UserId,
                user.PersonId,
                user.RoleId,
                jwt.Key,
                jwt.Issuer,
                jwt.Audience,
                jwt.ExpiresInMinutes
            );

            var refreshToken = await _refreshTokenService.CreateTokenAsync(
                user.UserId,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString()
            );

            if (string.IsNullOrWhiteSpace(refreshToken))
                return StatusCode(500, "Failed to create refresh token.");

            // ✅ Set both tokens in HttpOnly cookies
            SetAccessTokenCookie(accessToken, jwt.ExpiresInMinutes);
            SetRefreshTokenCookie(refreshToken);

            // ❌ Do NOT return tokens in body anymore
            return Ok(new
            {
                message = "Login successful",
                accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(jwt.ExpiresInMinutes),
                userId = user.UserId,
                entityId = user.PersonId,
                roleId = user.RoleId,
                role = user.RoleText
            });
        }
    }
}