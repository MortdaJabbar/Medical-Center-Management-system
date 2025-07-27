using AutoMapper;
using MCMSAPI.dtos;
using MCMSBussinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    
    [Route("api/Appointemnts")]
    [ApiController]
 
    public class AppointmentsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public AppointmentsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [Authorize(Roles ="Staff")]
        [HttpPost("add/")]
        public async Task<IActionResult> AddAppointment([FromBody] AddUpdateAppintmentdto dto)
        {
            var appointment = new Appointment
            {
                PatientID = dto.PatientID,
                DoctorID = dto.DoctorID,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = TimeOnly.Parse(dto.AppointmentTime),
                Paid= dto.Paid,
                Reason = dto.Reason,
                Status = dto.Status,
                Notes = dto.Notes
            };

            bool result = await appointment.AddNewAppointmentAsync();
            return result ? Ok("Appointment added successfully.") : BadRequest("Failed to add appointment.");
        }

        // PUT: api/Appointments/update
        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AddUpdateAppintmentdto dto)
        {
            Appointment? Appoint = await Appointment.FindByIdAsync(id);

            if (Appoint == null) return NotFound("No Appointment With  id");

            

            _mapper.Map(dto, Appoint);
            bool result = await Appoint.UpdateAppointmentAsync();
            return result ? Ok("Appointment updated successfully.") : NotFound("Appointment not found.");
        }

        // GET: api/Appointments/get/{id}
        [Authorize(Roles = "Staff")]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await Appointment.FindByIdAsync(id);
            return appointment != null ? Ok(appointment.DTO) : NotFound("Appointment not found.");
        }

        // GET: api/Appointments/get-all?page=1&size=10
        [HttpGet("get-all")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await Appointment.GetAllAsync();
            return Ok(appointments.Select(a => a.DTO));
        }

        // DELETE: api/Appointments/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await Appointment.DeleteAppointmentAsync(id);
            return result ? Ok("Appointment deleted.") : NotFound("Appointment not found.");
        }

        // GET: api/Appointments/by-patient/{patientId}
        [Authorize(Roles = "Patient")]
        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(Guid patientId)
        {
            var appointments = await Appointment.GetByPatientIdAsync(patientId);
            return Ok(appointments);
        }


        [Authorize(Roles = "Staff")]
        [HttpGet("patientSummary")]
        public async Task<IActionResult> GetPatientsSummaries()
        {
            var patients = await Patient.GetPatientSummariesAsync();
            return Ok(patients);
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("DoctorSummary")]
        public async Task<IActionResult> GetDoctorsSummaries()
        {
            var doctors = await Doctor.GetDoctorSummariesAsync();
            return Ok(doctors);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("WithDetails")]
        public async Task<IActionResult> GetAppointmentsWithDetails()
        {
            var appointments = await Appointment.GetAppointmentsWithDetailsAsync();
            return Ok(appointments);
        }

    }

}
