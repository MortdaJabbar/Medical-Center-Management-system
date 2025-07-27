using MCMSAPI.dtos;
using MCMSBussinessLogic;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
    
    public class UsersController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            List< UserAccount > allusers = await UserAccount.GetAllAsync();
      
           
            return Ok(allusers);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await UserAccount.FindByIDAsync(id);
            return user == null ? NotFound() : Ok(user.DTO);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Update/{UserId}")]
        public async Task<IActionResult> Update(Guid UserId,[FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await UserAccount.FindByIDAsync(UserId);
            if (user == null) return NotFound();

            user.Email = dto.Email;
            
            user.IsActive = dto.IsActive;
            user.Is2FAEnabled = dto.is2FAEnable;
            

            bool updated = await user.UpdateAsync();
            return updated ? Ok("User updated successfully.") : StatusCode(500, "Failed to update user.");
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool deleted = await UserAccount.DeleteAccountAsync(id);
            return deleted ? Ok("User deleted successfully.") : NotFound("User not found.");
        }
      
        [HttpPut("change-password/{UserId}")]
        public async Task<IActionResult> ChangePassword(Guid UserId ,[FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await UserAccount.FindByIDAsync(UserId);
            if (user == null) return NotFound("User not found.");

            bool changed = await user.ChangePasswordAsync(dto.OldPassword, dto.NewPassword);
            return changed ? Ok("Password changed successfully.") : BadRequest("Current password is incorrect Or Something went wrong try again.");
        }
        [HttpPut("2FA/{UserId}")]
        public async Task<IActionResult> Change2fa(Guid UserId, [FromBody] bool Is2faEnable)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await UserAccount.FindByIDAsync(UserId);
            if (user == null) return NotFound("User not found.");
            user.Is2FAEnabled = Is2faEnable;
            bool changed = await user.UpdateAsync();
            return changed ? Ok("2fa changed successfully.") : BadRequest(ModelState);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("detailed")]
        public   async Task<ActionResult<List<UserAccountDetailsDto>>> GetAllUsers()
        {
            var users = await UserAccount.GetAllUserAccountsAsync();
            return Ok(users);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("unregistered-patients")]
        public async Task<ActionResult<List<PatientWithoutAccountDto>>> GetUnregisteredPatients()
        {
            var result = await UserAccount.GetPatientsWithoutAccountAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("unregistered-doctors")]
        public async Task<ActionResult<List<DoctorWithoutAccountDto>>> GetUnregisteredDoctors()
        {
            var result = await UserAccount.GetDoctorsWithoutAccountAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("unregistered-pharmacists")]
        public async Task<ActionResult<List<PharmacistWithoutAccountDto>>> GetUnregisteredPharmacists()
        {
            var result = await UserAccount.GetPharmacistsWithoutAccountAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("unregistered-staff")]
        public async Task<ActionResult<List<StaffWithoutAccountDto>>> GetUnregisteredStaff()
        {
            var result = await UserAccount.GetStaffWithoutAccountAsync();
            return Ok(result);
        }


    }

}
