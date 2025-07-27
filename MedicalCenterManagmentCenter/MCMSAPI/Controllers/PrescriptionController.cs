using AutoMapper;
using MCMSAPI.dtos;
using MCMSBussinessLogic;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace MCMSAPI.Controllers
{
    
    [Route("api/Prescriptions")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public PrescriptionsController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddUpdatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<Prescription>(dto);
            bool added = await entity.AddNewPrescriptionAsync();

            return added ? Ok(entity.PrescriptionID) : BadRequest("Cannot add prescription.");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await Prescription.FindByIdAsync(id);
            if (existing == null) return NotFound("Prescription not found.");
            existing.PrescriptionDate = dto.PrescriptionDate;
            existing.Refills = dto.refills;
            existing.MedicationID = dto.MedicationID;
            existing.Instructions = dto.instructions;
           

             
            bool updated = await existing.UpdatePrescriptionAsync();

            return updated ? Ok("Updated.") : BadRequest("Update failed.");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await Prescription.FindByIdAsync(id);
            return result != null ? Ok(result.DTO) : NotFound();
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await Prescription.GetAllAsync();
            return Ok(list.Select(p => p.DTO));
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int size = 10)
        {
            var list = await Prescription.GetPagedAsync(page, size);
            return Ok(list.Select(p => p.DTO));
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await Prescription.DeleteByIdAsync(id);
            return deleted ? Ok("Deleted.") : NotFound("Not found.");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("detailed")]
        public async Task<ActionResult<List<PrescriptionDetailsDto>>> GetDetailedPrescriptions()
        {
            var prescriptions = await Prescription.GetAllPrescriptionsWithNamesAsync();
            return Ok(prescriptions);
        }



    }

}
