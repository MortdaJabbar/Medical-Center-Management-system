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
        [Authorize(Roles ="Admin")]
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

            // رجّع رد للمستخدم
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

            // فعل الحساب
            bool activated = await UserAccountData.ActivateUserAsync(userId);
            if (!activated)
                return StatusCode(500, "Failed to activate account.");

            // علم الرمز كـ مستخدم
            await EmailVerificationData.MarkAsUsedAsync(token);

            return Ok("Email verified successfully.");
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var MyUser = await UserAccount.FindByEmailAsync(dto.Email);

            if (MyUser == null)
                return Unauthorized("Invalid credentials.");

            if (!MyUser.IsActive)
                return Unauthorized("Account is not activated.");

            if (!PasswordHelper.VerifyPassword(dto.Password, MyUser.PasswordHash))
                return Unauthorized("Email Or Password Is Not Correct Please Contact Your Admin.");

            if (!MyUser.Is2FAEnabled)
            {
                var token = JwtHelper.GenerateJwtToken(MyUser.UserId, MyUser.PersonId, MyUser.RoleId,

                   "STORNGKEY&%#@^$$!@#&%*#@ABCSIUHUDTYE^99",
                   "MCMS",
                   "MCMSUsers",
                     120);
                return Ok(new
                {
                    token,
                    userId = MyUser.UserId,
                    entityId = MyUser.PersonId,
                    roleId = MyUser.RoleId,
                    role = MyUser.RoleText
                });

            }

            // لو 2FA مفعل → أرسل الكود للإيميل
            Task.Run(()=>EmailSender.SendTwoFactorCodeAsync(MyUser.Email, MyUser.UserId));

            return Ok(new
            {
                MyUser.UserId,
                message = "2FA code sent. Please verify to complete login."
            });



        }
        
        [HttpPost("ChangePassword/{UserId}")]
        public async Task<IActionResult> ChanagePassword(Guid UserId, [FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var MyUser = await UserAccount.FindByIDAsync(UserId);

            if (MyUser == null)
                return BadRequest("Invalid credentials.No User With this Id");

            bool Updated = await MyUser.ChangePasswordAsync(dto.OldPassword, dto.NewPassword);

            return Updated ? Ok("Password Updated Succesffuly") : BadRequest("Failed To Update Password");

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


            var MyUser = await UserAccount.FindByIDAsync(userId); // تعمل نفس FindByEmailAsync
            if (MyUser == null) return NotFound("User not found.");

            var token = JwtHelper.GenerateJwtToken(MyUser.UserId, MyUser.PersonId, MyUser.RoleId,

                   _config["Jwt:Key"],
                   _config["Jwt:Issuer"],
                   _config["Jwt:Audience"],
                   int.Parse(_config["Jwt:ExpiresInMinutes"]));
            return Ok(new
            {
                token,
                userId = MyUser.UserId,
                entityId = MyUser.PersonId,
                roleId = MyUser.RoleId,
                role = MyUser.RoleText

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
                return Ok("If an account exists, a reset link has been sent."); // Don't reveal existence

          Task.Run (()=> EmailSender.SendPasswordResetEmailAsync(email, user.UserId));
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

            await PasswordResetData.DeleteTokenAsync(dto.Token); // أمنياً نحذف التوكن

            return Ok("Password has been reset successfully.");



        }

    }
}