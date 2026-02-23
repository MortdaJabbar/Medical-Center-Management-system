using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPatientService _patientService;

        public InvoicesController(IInvoiceService invoiceService, IPatientService patientService)
        {
            _invoiceService = invoiceService;
            _patientService = patientService;
        }

        // GET: api/Invoices/detailed
        [Authorize(Roles = "Staff")]
        [HttpGet("detailed")]
        public async Task<ActionResult<List<InvoiceDetailsDto>>> GetAll()
        {
            var data = await _invoiceService.GetAllAsync();
            return Ok(data);
        }

        // GET: api/Invoices/by-patient/{patientId}
        [Authorize(Roles = "Patient")]
        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<List<PatientInvoiceDto>>> GetByPatient(Guid patientId,[FromServices] IAuthorizationService authorizationService)
        {
            // 1️⃣ Load patient
            var patient = await _patientService.FindByIdAsync(patientId);

            if (patient == null)
                return NotFound();

            // 2️⃣ Ownership check
            var authResult = await authorizationService.AuthorizeAsync(
                User,
                patient.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();

            var result = await _invoiceService.GetByPatientIdAsync(patientId);
            return Ok(result);
        }

        // POST: api/Invoices
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddInvoiceDto dto)
        {
            var success = await _invoiceService.InsertAsync(dto);
            return success ? Ok() : BadRequest("Insert failed.");
        }

        // PUT: api/Invoices/5
        [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceDto dto)
        {
            var success = await _invoiceService.UpdateAsync(id, dto);
            return success ? Ok() : NotFound();
        }

        // DELETE: api/Invoices/5
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _invoiceService.DeleteAsync(id);
            return success ? Ok() : NotFound();
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("unpaid-tests")]
        public async Task<ActionResult<List<UnpaidServiceDto>>> GetUnpaidTests()
        {
            var result = await _invoiceService.GetUnpaidTestsAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("unpaid-appointments")]
        public async Task<ActionResult<List<UnpaidServiceDto>>> GetUnpaidAppointments()
        {
            var result = await _invoiceService.GetUnpaidAppointmentsAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("unpaid-prescriptions")]
        public async Task<ActionResult<List<UnpaidServiceDto>>> GetUnpaidPrescriptions()
        {
            var result = await _invoiceService.GetUnpaidPrescriptionsAsync();
            return Ok(result);
        }
    }

}
