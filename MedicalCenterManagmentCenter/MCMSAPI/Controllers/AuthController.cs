using MCMSBussinessLogic;
using MCMSDAL;
using Microsoft.AspNetCore.Mvc;
using MCMSAPI.Helper;
using MCMSAPI.dtos;
using Microsoft.AspNetCore.Authorization;

namespace MCMSAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        private (string Key, string Issuer, string Audience, int ExpiresInMinutes) GetJwtOptions()
        {
            string key = _config["Jwt:Key"] ?? "";
            string issuer = _config["Jwt:Issuer"] ?? "";
            string audience = _config["Jwt:Audience"] ?? "";
            string expiresStr = _config["Jwt:ExpiresInMinutes"] ?? "";

            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("JWT Key is missing. Set Jwt:Key in configuration/environment variables.");
            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT Issuer is missing. Set Jwt:Issuer in configuration/environment variables.");
            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT Audience is missing. Set Jwt:Audience in configuration/environment variables.");
            if (!int.TryParse(expiresStr, out int expiresInMinutes) || expiresInMinutes <= 0)
                throw new InvalidOperationException("JWT ExpiresInMinutes is invalid. Set Jwt:ExpiresInMinutes to a positive number.");

            return (key, issuer, audience, expiresInMinutes);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new UserAccount
            {
                PersonId = dto.PersonId,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                RoleId = dto.RoleId,
            };

            bool created = await user.RegisterAsync();
            if (!created)
                return BadRequest("Email already exists.");

            // إرسال رمز التحقق بالبريد
            Task.Run(() => EmailSender.SendVerificationEmailAsync(user.Email, user.UserId));

            return Ok(new { user.UserId, message = "Account created. Please check your email to verify." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var result = await EmailVerificationData.FindByTokenAsync(token);

            if (result == null)
                return NotFound("Invalid token.");

            var (userId, isUsed, expiry) = result.Value;

            if (isUsed)
                return BadRequest("Token already used.");

            if (expiry < DateTime.UtcNow)
                return BadRequest("Token has expired.");

            bool activated = await UserAccountData.ActivateUserAsync(userId);
            if (!activated)
                return StatusCode(500, "Failed to activate account.");

            await EmailVerificationData.MarkAsUsedAsync(token);

            return Ok("Email verified successfully.");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var myUser = await UserAccount.FindByEmailAsync(dto.Email);

            if (myUser == null)
                return Unauthorized("Invalid credentials.");

            if (!myUser.IsActive)
                return Unauthorized("Account is not activated.");

            if (!PasswordHelper.VerifyPassword(dto.Password, myUser.PasswordHash))
                return Unauthorized("Email Or Password Is Not Correct Please Contact Your Admin.");

            // ✅ Always use config/env (no hardcoding)
            var jwt = GetJwtOptions();

            if (!myUser.Is2FAEnabled)
            {
                var csvPath = _config["RefreshTokens:CsvPath"];
                var hashKey = _config["RefreshTokens:HashKey"];
                var expiresDays = int.Parse(_config["RefreshTokens:ExpiresInDays"] ?? "7");

                var issue = await RefreshTokenService.IssueAsync(
                    myUser.UserId,
                    csvPath!,
                    hashKey!,
                    expiresDays,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers.UserAgent.ToString()
                );

                var accessToken = JwtHelper.GenerateJwtToken(
                    myUser.UserId,
                    myUser.PersonId,
                    myUser.RoleId,
                    jwt.Key,
                    jwt.Issuer,
                    jwt.Audience,
                    jwt.ExpiresInMinutes
                );

                return Ok(new
                {
                    accessToken,
                    refreshToken = issue.RefreshToken,
                    accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(jwt.ExpiresInMinutes),
                    refreshTokenExpiresAtUtc = issue.ExpiresAtUtc,
                    userId = myUser.UserId,
                    entityId = myUser.PersonId,
                    roleId = myUser.RoleId,
                    role = myUser.RoleText
                });
            }

            // لو 2FA مفعل → أرسل الكود للإيميل
            Task.Run(() => EmailSender.SendTwoFactorCodeAsync(myUser.Email, myUser.UserId));

            return Ok(new
            {
                myUser.UserId,
                message = "2FA code sent. Please verify to complete login."
            });
        }

        [HttpPost("ChangePassword/{UserId}")]
        public async Task<IActionResult> ChanagePassword(Guid UserId, [FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var myUser = await UserAccount.FindByIDAsync(UserId);

            if (myUser == null)
                return BadRequest("Invalid credentials.No User With this Id");

            bool updated = await myUser.ChangePasswordAsync(dto.OldPassword, dto.NewPassword);

            return updated ? Ok("Password Updated Succesffuly") : BadRequest("Failed To Update Password");
        }

        [HttpPost("confirm-2fa")]
        public async Task<IActionResult> Confirm2FA(Guid userId, string code)
        {
            var result = await TwoFactorCodeData.GetLatestCodeAsync(userId);
            if (result == null) return BadRequest("No 2FA code found.");

            var (storedCode, expiry, isUsed) = result.Value;

            if (isUsed) return BadRequest("Code already used.");
            if (expiry < DateTime.UtcNow) return BadRequest("Code expired.");
            if (storedCode != code) return BadRequest("Invalid code.");

            await TwoFactorCodeData.MarkAsUsedAsync(userId, code);

            var myUser = await UserAccount.FindByIDAsync(userId);
            if (myUser == null) return NotFound("User not found.");

            var jwt = GetJwtOptions();

            var csvPath = _config["RefreshTokens:CsvPath"];
            var hashKey = _config["RefreshTokens:HashKey"];
            var expiresDays = int.Parse(_config["RefreshTokens:ExpiresInDays"] ?? "7");

            var issue = await RefreshTokenService.IssueAsync(
                myUser.UserId,
                csvPath!,
                hashKey!,
                expiresDays,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString()
            );

            var accessToken = JwtHelper.GenerateJwtToken(
                myUser.UserId,
                myUser.PersonId,
                myUser.RoleId,
                jwt.Key,
                jwt.Issuer,
                jwt.Audience,
                jwt.ExpiresInMinutes
            );

            return Ok(new
            {
                accessToken,
                refreshToken = issue.RefreshToken,
                accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(jwt.ExpiresInMinutes),
                refreshTokenExpiresAtUtc = issue.ExpiresAtUtc,
                userId = myUser.UserId,
                entityId = myUser.PersonId,
                roleId = myUser.RoleId,
                role = myUser.RoleText
            });
        }

        [HttpGet("profile/{personId}")]
        public async Task<ActionResult<PersonProfileDto>> GetProfile(Guid personId)
        {
            var result = await Person.GetProfileAsync(personId);
            if (result == null)
                return NotFound("Person not found");

            return Ok(result);
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            var user = await UserAccount.FindByEmailAsync(email);
            if (user == null)
                return Ok("If an account exists, a reset link has been sent.");

            Task.Run(() => EmailSender.SendPasswordResetEmailAsync(email, user.UserId));
            return Ok("If an account exists, a reset link has been sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var record = await PasswordResetData.GetByTokenAsync(dto.Token);
            if (record == null || record.expiry < DateTime.UtcNow)
                return BadRequest("Invalid or expired token.");

            string hashedPassword = PasswordHelper.HashPassword(dto.NewPassword);

            UserAccount myUser = await UserAccount.FindByIDAsync(record.userId) ?? null;
            if (myUser == null) return NotFound("No User with this ID");

            myUser.PasswordHash = hashedPassword;

            bool updated = await myUser.ResetPassword();
            if (!updated)
                return StatusCode(500, "Failed to update password.");

            await PasswordResetData.DeleteTokenAsync(dto.Token);

            return Ok("Password has been reset successfully.");
        }
    }
}