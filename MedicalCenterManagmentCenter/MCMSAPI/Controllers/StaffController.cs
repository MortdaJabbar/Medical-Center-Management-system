using AutoMapper;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStaffService _staffService;

        public StaffController(IMapper mapper, IStaffService staffService)
        {
            _mapper = mapper;
            _staffService = staffService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddStaff([FromBody] AddUpdateStaffDto dto)
        {
            var staff = _mapper.Map<Staff>(dto);
            var newId = await _staffService.CreateAsync(staff);
            return newId != null ? Ok(newId.Value) : BadRequest("Staff already exists or could not be added.");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] AddUpdateStaffDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { errors });
            }

            var staff = await _staffService.FindByIdAsync(id);
            if (staff == null) return NotFound("Staff not found.");

            _mapper.Map(dto, staff); // يحدّث كل بيانات الشخص والموظف
            bool updated = await _staffService.UpdateAsync(staff);

            return updated ? Ok("Updated successfully.") : StatusCode(500, "Could not update staff.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(Guid id)
        {
            var staff = await _staffService.FindByIdAsync(id);
            return staff != null ? Ok(staff.DTO) : NotFound("Staff not found.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllStaff()
        {
            var staffList = await _staffService.GetAllAsync();
            return Ok(staffList.Select(s => s.DTO));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("summary")]
        public async Task<IActionResult> GetStaffSummaries()
        {
            var summaries = await _staffService.GetSummariesAsync();
            return Ok(summaries);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStaff(Guid id)
        {
            var staff = await _staffService.FindByIdAsync(id);
            if (staff == null) { return NotFound("Staff not found."); }
            bool deleted = await _staffService.DeleteAsync(id, staff.PersonId);
            return deleted ? Ok("Deleted successfully.") : StatusCode(500, "Internal Error in our servers  ");
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("staff-stats")]
        public async Task<ActionResult<StaffDashboardStatsDto>> GetStaffDashboardStats()
        {
            var stats = await _staffService.GetStaffDashboardStatsAsync();
            return Ok(stats);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-stats")]
        public async Task<ActionResult<AdminDashboardStatsDto>> GetAdminStats()
        {
            var stats = await _staffService.GetAdminDashboardStatsAsync();
            return Ok(stats);
        }

    }

}
