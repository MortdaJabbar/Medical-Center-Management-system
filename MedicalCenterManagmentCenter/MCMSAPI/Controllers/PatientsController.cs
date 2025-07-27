using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MCMSBussinessLogic;
using MCMSDAL;
using MCMSAPI.dtos.Mapper;
using AutoMapper;
using System.Text.RegularExpressions;
using MCMSAPI.dtos.PatientsDto;
using Microsoft.AspNetCore.Authorization;
namespace MCMSAPI.Controllers
{
    [Route("api/Patients")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
   
        private readonly IMapper _mapper;

        public PatientsController(IMapper mapper)
        {
            _mapper = mapper;
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
            
            var result = await patient.AddNewPatientAsync();

            return result ? Ok(patient.DTO.PatientId) : BadRequest("Failed to add patient");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(Guid id)
        {
            var patient = await Patient.FindPatientByIdAsync(id);
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


            Patient? Patient = await Patient.FindPatientByIdAsync(PatientId);           

            if ( Patient==null) return NotFound("No Patient With this patient id");
            

            _mapper.Map(dto, Patient);

            bool updated = await Patient.UpdatePatientAsync();

            return updated ? Ok("Patient updated") : NotFound("Patient or person not found");
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{patientId}/person/{personId}")]
        public async Task<IActionResult> DeletePatient(Guid patientId, Guid personId)
        {
            bool deleted = await Patient.DeletePatientByIdAsync(patientId, personId);
            return deleted ? Ok("Deleted successfully") : NotFound("Could not delete patient");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPatients( )
        {
            var list = await Patient.GetAllPatientsAsync();
            return Ok(list.Select(p => p.DTO));
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("appointments/{id}")]
        public async Task<ActionResult<List<AppointmentPatientDto>>> GetAppointments(Guid id)
        {

            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");
            
            var result = await Patient.GetPatientAppoitments(id);
            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("prescriptions/{id}")]
        public async Task<ActionResult<List<PrescriptionPatientDto>>> GetPrescriptions(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");
            

            var result = await Patient.GetPrescriptionsByPatientIdAsync(id);
            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("tests/{id}")]
        public async Task<ActionResult<List<TestPatientsDto>>> GetTests(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");
            

            var result = await Patient.GetTestsByPatientIdAsync(id);
            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("dashboard/{id}")]
        public async Task<ActionResult<PatientDashboardDto>> GetDashboard(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Patient ID");
             

            var result = await Patient.GetPatientDashboardStatsAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("Invoices/{patientId}")]
        public async Task<IActionResult> GetByPatient(Guid patientId)
        {
             

            var payments = await Patient.GetInvoicesForPatientAsync(patientId);
            return Ok(payments);
        }

       
    }
}
