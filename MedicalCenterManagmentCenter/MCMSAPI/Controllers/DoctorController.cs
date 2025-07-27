using AutoMapper;
using MCMSAPI.dtos;
using MCMSAPI.dtos.DoctorDto;
using MCMSBussinessLogic;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [Route("api/Doctors")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public DoctorsController(IMapper mapper)
        {
            _mapper = mapper;
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
            bool isAdded = await doctor.AddNewDoctorAsync();

            return isAdded ? Ok(doctor.DTO.DoctorId) : BadRequest("Doctor already exists or cannot be added.");
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

            Doctor? Doctor = await Doctor.FindDoctorByIdAsync(id);
            if (Doctor == null) return NotFound("Doctor ID mismatch Or Doctor With this id.");

            _mapper.Map(doctorDto, Doctor);

            bool updated = await Doctor.UpdateDoctorAsync();

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
            var doctor = await Doctor.FindDoctorByIdAsync(id);
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
            var doctors = await Doctor.GetAllDoctorsAsync();
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
            bool deleted = await Doctor.DeleteDoctorByIdAsync(doctorId, personId);
            return deleted ? Ok("Deleted successfully.") : NotFound("Doctor not found or could not be deleted.");
        }
        [Authorize(Roles ="Doctor")]
        [HttpGet("appointments/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctorId(Guid doctorId)
        {
            
            try
            {
                var results = await Doctor.GetAppointmentsByDoctorIdAsync(doctorId);

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
        public async Task<IActionResult> GetTestsByDoctorId(Guid doctorId)
        {
            
            var tests = await Test.GetTestsByDoctorIdAsync(doctorId);


            return Ok(tests);

        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("Prescriptions/{doctorId}")]
        public async Task<IActionResult> GetPrescriptionsByDoctorId(Guid doctorId)
        {
            
            try
            {
                var prescriptions = await Doctor.GetPrescriptionsByDoctorIdAsync(doctorId);

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
        public async Task<IActionResult> GetDashboardStats(Guid doctorId)
        {
            
            try
            {
                var stats = await Doctor.GetDashboardStatsAsync(doctorId);

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