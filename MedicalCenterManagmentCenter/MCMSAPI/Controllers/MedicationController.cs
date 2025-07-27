using AutoMapper;
using MCMSBussinessLogic;
using Microsoft.AspNetCore.Mvc;
using MCMSAPI.dtos;
using Microsoft.AspNetCore.Authorization;
namespace MCMSAPI.Controllers
{

    [Route("api/Medications")]
    
    [ApiController]
    public class MedicationsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public MedicationsController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [Authorize(Roles = "Staff")]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddUpdateMedicationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var medication = _mapper.Map<Medication>(dto);
            bool added = await medication.AddNewMedicationAsync();

            return added ? Ok("Medication added.") : BadRequest("Failed to add.");
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AddUpdateMedicationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await Medication.FindByIdAsync(id);
            if (existing == null) return NotFound("Medication not found.");

            _mapper.Map(dto, existing);
            bool updated = await existing.UpdateMedicationAsync();

            return updated ? Ok("Updated successfully.") : BadRequest("Failed to update.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medication = await Medication.FindByIdAsync(id);
            return medication != null ? Ok(medication.DTO) : NotFound("Not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var meds = await Medication.GetAllAsync();
            
            return Ok(meds);
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await Medication.DeleteMedicationAsync(id);
            return deleted ? Ok("Deleted.") : NotFound("Medication not found.");
        }
    }

}
