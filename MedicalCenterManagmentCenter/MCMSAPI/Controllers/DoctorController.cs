using AutoMapper;
using MCMSAPI.dtos;
using MCMSAPI.dtos.DoctorDto;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [Route("api/Doctors")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDoctorService _doctorService;
        private readonly ITestService _testService;

        public DoctorsController(IMapper mapper, IDoctorService doctorService, ITestService testService)
        {
            _mapper = mapper;
            _doctorService = doctorService;
            _testService = testService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddDoctor([FromBody] AddUpdateDoctorDto addDoctorDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors = errors });
            }

          
            var doctor = _mapper.Map<Doctor>(addDoctorDto);
            var doctorId = await _doctorService.CreateAsync(doctor);

            return doctorId != null ? Ok(doctorId.Value) : BadRequest("Doctor already exists or cannot be added.");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] AddUpdateDoctorDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors = errors });
            }

            Doctor? Doctor = await _doctorService.FindByIdAsync(id);
            if (Doctor == null) return NotFound("Doctor ID mismatch Or Doctor With this id.");

            _mapper.Map(doctorDto, Doctor);

            bool updated = await _doctorService.UpdateAsync(Doctor);

            return updated ? Ok("Updated successfully.") : NotFound("Doctor not found.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(Guid id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors = errors });
            }
            var doctor = await _doctorService.FindByIdAsync(id);
            return doctor != null ? Ok(doctor.DTO) : NotFound("Doctor not found.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllDoctors(int page = 1, int size = 10)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors = errors });
            }
            var doctors = await _doctorService.GetAllAsync();
            var dtoList = doctors.ConvertAll(d => d.DTO);
            return Ok(dtoList);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{doctorId}/person/{personId}")]
        public async Task<IActionResult> DeleteDoctor( Guid doctorId,  Guid personId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors = errors });
            }
            bool deleted = await _doctorService.DeleteAsync(doctorId, personId);
            return deleted ? Ok("Deleted successfully.") : NotFound("Doctor not found or could not be deleted.");
        }
        [Authorize(Roles ="Doctor")]
        [HttpGet("appointments/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctorId(Guid doctorId, [FromServices] IAuthorizationService authorizationService)
        {

            var doctor = await _doctorService.FindByIdAsync(doctorId);

            if (doctor == null)
                return NotFound("Doctor not found.");

            // 2️⃣ Ownership check using PersonId
            var auth = await authorizationService.AuthorizeAsync(
                User,
                doctor.DTO.Person.PersonId,   // VERY IMPORTANT
                "OwnerOnly");

            if (!auth.Succeeded)
                return Forbid();
            try
            {
                var results = await _doctorService.GetAppointmentsByDoctorIdAsync(doctorId);

                if (results == null || results.Count == 0)
                    return Ok("No appointments found for the specified doctor.");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("tests/{doctorId}")]
        public async Task<IActionResult> GetTestsByDoctorId(Guid doctorId ,[FromServices] IAuthorizationService authorizationService)
        {
            var doctor = await _doctorService.FindByIdAsync(doctorId);
            if (doctor == null)
                return NotFound();

            var auth = await authorizationService.AuthorizeAsync(
                User,
                doctor.DTO.Person.PersonId,
                "OwnerOnly");

            if (!auth.Succeeded)
                return Forbid();
            var tests = await _testService.GetByDoctorIdAsync(doctorId);


            return Ok(tests);

        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("Prescriptions/{doctorId}")]
        public async Task<IActionResult> GetPrescriptionsByDoctorId(Guid doctorId,
    [FromServices] IAuthorizationService authorizationService)
        {
            var doctor = await _doctorService.FindByIdAsync(doctorId);
            if (doctor == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                doctor.DTO.Person.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();
            try
            {
                var prescriptions = await _doctorService.GetPrescriptionsByDoctorIdAsync(doctorId);

                if (prescriptions == null || prescriptions.Count == 0)
                    return Ok("No prescriptions found for the specified doctor.");
                

                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("dashboard/{doctorId}")]
        public async Task<IActionResult> GetDashboardStats(Guid doctorId,
    [FromServices] IAuthorizationService authorizationService)
        {
            var doctor = await _doctorService.FindByIdAsync(doctorId);
            if (doctor == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                doctor.DTO.Person.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();

            try
            {
                var stats = await _doctorService.GetDashboardStatsAsync(doctorId);

                if (stats == null)
                    return Ok("No stats found for this doctor.");

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        

    }
}