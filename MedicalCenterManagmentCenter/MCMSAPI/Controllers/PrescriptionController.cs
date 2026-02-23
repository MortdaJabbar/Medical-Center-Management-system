using AutoMapper;
using MCMSAPI.dtos;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace MCMSAPI.Controllers
{
    
    [Route("api/Prescriptions")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(IMapper mapper, IPrescriptionService prescriptionService)
        {
            _mapper = mapper;
            _prescriptionService = prescriptionService;
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddUpdatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<Prescription>(dto);
            var newId = await _prescriptionService.CreateAsync(entity);

            return newId != null ? Ok(newId.Value) : BadRequest("Cannot add prescription.");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _prescriptionService.FindByIdAsync(id);
            if (existing == null) return NotFound("Prescription not found.");
            existing.PrescriptionDate = dto.PrescriptionDate;
            existing.Refills = dto.refills;
            existing.MedicationID = dto.MedicationID;
            existing.Instructions = dto.instructions;
           

             
            bool updated = await _prescriptionService.UpdateAsync(existing);

            return updated ? Ok("Updated.") : BadRequest("Update failed.");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _prescriptionService.FindByIdAsync(id);
            return result != null ? Ok(result.DTO) : NotFound();
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _prescriptionService.GetAllAsync();
            return Ok(list.Select(p => p.DTO));
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int size = 10)
        {
            var list = await _prescriptionService.GetPagedAsync(page, size);
            return Ok(list.Select(p => p.DTO));
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _prescriptionService.DeleteAsync(id);
            return deleted ? Ok("Deleted.") : NotFound("Not found.");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("detailed")]
        public async Task<ActionResult<List<PrescriptionDetailsDto>>> GetDetailedPrescriptions()
        {
            var prescriptions = await _prescriptionService.GetDetailedAsync();
            return Ok(prescriptions);
        }

    }

}
