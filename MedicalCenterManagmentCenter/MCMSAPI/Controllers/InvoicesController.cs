using MCMSBussinessLogic;
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
        // GET: api/Invoices/detailed
        [Authorize(Roles = "Staff")]
        [HttpGet("detailed")]
        public async Task<ActionResult<List<InvoiceDetailsDto>>> GetAll()
        {
            var data = await Invoice.GetAllAsync();
            return Ok(data);
        }

        // GET: api/Invoices/by-patient/{patientId}
        [Authorize(Roles = "Patient")]
        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<List<PatientInvoiceDto>>> GetByPatient(Guid patientId)
        {
            var result = await Invoice.GetByPatientIdAsync(patientId);
            return Ok(result);
        }

        // POST: api/Invoices
        [Authorize(Roles = "Staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddInvoiceDto dto)
        {
            var success = await Invoice.InsertAsync(dto);
            return success ? Ok() : BadRequest("Insert failed.");
        }

        // PUT: api/Invoices/5
        [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceDto dto)
        {
            var success = await Invoice.UpdateAsync(id, dto);
            return success ? Ok() : NotFound();
        }

        // DELETE: api/Invoices/5
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await Invoice.DeleteAsync(id);
            return success ? Ok() : NotFound();
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("unpaid-tests")]
        public async Task<ActionResult<List<UnpaidServiceDto>>> GetUnpaidTests()
        {
            var result = await Invoice.GetUnpaidTestsAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("unpaid-appointments")]
        public async Task<ActionResult<List<UnpaidServiceDto>>> GetUnpaidAppointments()
        {
            var result = await Invoice.GetUnpaidAppointmentsAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("unpaid-prescriptions")]
        public async Task<ActionResult<List<UnpaidServiceDto>>> GetUnpaidPrescriptions()
        {
            var result = await Invoice.GetUnpaidPrescriptionsAsync();
            return Ok(result);
        }
    }

}
