using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MCMSBussinessLogic;
using MCMSAPI.dtos.Mapper;
using AutoMapper;
using System.Text.RegularExpressions;
using MCMSAPI.dtos.PatientsDto;
using Microsoft.AspNetCore.Authorization;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
namespace MCMSAPI.Controllers
{
    [Route("api/Patients")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
   
        private readonly IMapper _mapper;
        private readonly IPatientService _patientService;

        public PatientsController(IMapper mapper, IPatientService patientService)
        {
            _mapper = mapper;
            _patientService = patientService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddPatient([FromBody] AddUpdatePatientDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors = errors });
            }

            var patient = _mapper.Map<Patient>(dto);

            var newId = await _patientService.CreateAsync(patient);

            return newId != null ? Ok(newId.Value) : BadRequest("Failed to add patient");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(Guid id)
        {
            var patient = await _patientService.FindByIdAsync(id);
            return (patient == null) ? NotFound() : Ok(patient.DTO);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePatient(Guid PatientId , [FromBody] AddUpdatePatientDto dto)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors = errors });
            }


            Patient? Patient = await _patientService.FindByIdAsync(PatientId);

            if ( Patient==null) return NotFound("No Patient With this patient id");
            

            _mapper.Map(dto, Patient);

            bool updated = await _patientService.UpdateAsync(Patient);

            return updated ? Ok("Patient updated") : NotFound("Patient or person not found");
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{patientId}/person/{personId}")]
        public async Task<IActionResult> DeletePatient(Guid patientId, Guid personId)
        {
            bool deleted = await _patientService.DeleteAsync(patientId, personId);
            return deleted ? Ok("Deleted successfully") : NotFound("Could not delete patient");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPatients( )
        {
            var list = await _patientService.GetAllAsync();
            return Ok(list.Select(p => p.DTO));
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("appointments/{id}")]
        public async Task<ActionResult<List<AppointmentPatientDto>>> GetAppointments(Guid id,
    [FromServices] IAuthorizationService authorizationService)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");

            var patient = await _patientService.FindByIdAsync(id);

            if (patient == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                patient.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();

            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");
            
            var result = await _patientService.GetAppointmentsAsync(id);
            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("prescriptions/{id}")]
        public async Task<ActionResult<List<PrescriptionPatientDto>>> GetPrescriptions(Guid id,
    [FromServices] IAuthorizationService authorizationService)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");

            var patient = await _patientService.FindByIdAsync(id);

            if (patient == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                patient.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();
           

            var result = await _patientService.GetPrescriptionsAsync(id);
            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("tests/{id}")]
        public async Task<ActionResult<List<TestPatientsDto>>> GetTests(Guid id,
    [FromServices] IAuthorizationService authorizationService)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");

            var patient = await _patientService.FindByIdAsync(id);

            if (patient == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                patient.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();

            var result = await _patientService.GetTestsAsync(id);
            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("dashboard/{id}")]
        public async Task<ActionResult<PatientDashboardDto>> GetDashboard(Guid id,
    [FromServices] IAuthorizationService authorizationService)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");

            var patient = await _patientService.FindByIdAsync(id);

            if (patient == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                patient.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();

            var result = await _patientService.GetDashboardStatsAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("Invoices/{patientId}")]
        public async Task<IActionResult> GetByPatient(Guid patientId,
    [FromServices] IAuthorizationService authorizationService)
        {
            if (patientId == Guid.Empty)
                return BadRequest("Invalid Patient ID");

            var patient = await _patientService.FindByIdAsync(patientId);

            if (patient == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                patient.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();
            var payments = await _patientService.GetInvoicesAsync(patientId);
            return Ok(payments);
        }

       
    }
}
