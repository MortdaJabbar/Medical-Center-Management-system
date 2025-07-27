using AutoMapper;
using MCMSAPI.dtos;
using MCMSBussinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [ApiController]
    [Route("api/Inventory")]
    
    public class InventoryController : ControllerBase
    {
        private readonly IMapper _mapper;

        public InventoryController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [Authorize(Roles = "Staff")]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddUpdateInventoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = _mapper.Map<Inventory>(dto);
            bool added = await inventory.AddNewInventoryAsync();
            return added ? Ok(inventory.InventoryID) : BadRequest("Inventory creation failed.");
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AddUpdateInventoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await Inventory.FindInventoryByIdAsync(id);
            if (existing == null) return NotFound("Inventory not found.");

            _mapper.Map(dto, existing);
            bool updated = await existing.UpdateInventoryAsync();
            return updated ? Ok("Updated successfully.") : BadRequest("Update failed.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await Inventory.FindInventoryByIdAsync(id);
            return item != null ? Ok(item.DTO) : NotFound("Inventory not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await Inventory.GetAllInventoryAsync();
            return Ok(list);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("AllDetails")]
        public async Task<IActionResult> GetAllDetails()
        {
            var list = await Inventory.GetInventoryDetailsAsync();
            return Ok(list);
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await Inventory.DeleteInventoryByIdAsync(id);
            return deleted ? Ok("Deleted successfully.") : NotFound("Inventory not found.");
        }
    }

}
