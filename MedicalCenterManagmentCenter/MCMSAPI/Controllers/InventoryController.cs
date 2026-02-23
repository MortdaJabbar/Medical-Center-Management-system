using AutoMapper;
using MCMSAPI.dtos;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/Inventory")]
    
    public class InventoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IInventoryService _inventoryService;

        public InventoryController(IMapper mapper, IInventoryService inventoryService)
        {
            _mapper = mapper;
            _inventoryService = inventoryService;
        }
        [Authorize(Roles = "Staff")]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddUpdateInventoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = _mapper.Map<Inventory>(dto);
            var addedId = await _inventoryService.CreateAsync(inventory);
            return addedId != null ? Ok(addedId.Value) : BadRequest("Inventory creation failed.");
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AddUpdateInventoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _inventoryService.FindByIdAsync(id);
            if (existing == null) return NotFound("Inventory not found.");

            _mapper.Map(dto, existing);
            bool updated = await _inventoryService.UpdateAsync(existing);
            return updated ? Ok("Updated successfully.") : BadRequest("Update failed.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _inventoryService.FindByIdAsync(id);
            return item != null ? Ok(item.DTO) : NotFound("Inventory not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _inventoryService.GetAllAsync();
            return Ok(list);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("AllDetails")]
        public async Task<IActionResult> GetAllDetails()
        {
            var list = await _inventoryService.GetAllDetailsAsync();
            return Ok(list);
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _inventoryService.DeleteAsync(id);
            return deleted ? Ok("Deleted successfully.") : NotFound("Inventory not found.");
        }
    }

}
