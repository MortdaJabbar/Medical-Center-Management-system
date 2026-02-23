using AutoMapper;
using MCMSAPI.dtos.PharmacistDto;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace MCMSAPI.Controllers
{
    [Route("api/Pharmacists")]
    [ApiController]
    [Authorize]
    public class PharmacistController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPharmacistService _pharmacistService;

        public PharmacistController(IMapper mapper, IPharmacistService pharmacistService)
        {
            _mapper = mapper;
            _pharmacistService = pharmacistService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddPharmacist([FromBody] AddUpdatePharmacistDto addPharmacistDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { errors });
            }

            var pharmacist = _mapper.Map<Pharmacist>(addPharmacistDto);
            var newId = await _pharmacistService.CreateAsync(pharmacist);

            return newId != null ? Ok(newId.Value) : BadRequest("Pharmacist already exists or cannot be added.");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePharmacist(Guid id, [FromBody] AddUpdatePharmacistDto pharmacistDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { errors });
            }

            Pharmacist? pharmacist = await _pharmacistService.FindByIdAsync(id);
            if (pharmacist == null) return NotFound("Pharmacist not found.");

            _mapper.Map(pharmacistDto, pharmacist);
            bool updated = await _pharmacistService.UpdateAsync(pharmacist);

            return updated ? Ok("Updated successfully.") : NotFound("Pharmacist could not be updated.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacistById(Guid id)
        {
            var pharmacist = await _pharmacistService.FindByIdAsync(id);
            return pharmacist != null ? Ok(pharmacist.DTO) : NotFound("Pharmacist not found.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPharmacists( )
        {
            var pharmacists = await _pharmacistService.GetAllAsync();
            var dtoList = pharmacists.ConvertAll(p => p.DTO);
            return Ok(dtoList);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{pharmacistId}")]
        public async Task<IActionResult> DeletePharmacist(Guid pharmacistId)
        {
            Pharmacist Pharmist = await _pharmacistService.FindByIdAsync(pharmacistId);
            if (Pharmist == null)
                return NotFound();
            bool deleted = await _pharmacistService.DeleteAsync(pharmacistId, Pharmist.PersonId);
            return deleted ? Ok("Deleted successfully.") : StatusCode(500, "Internal Error in our servers  ");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("pharmacy-stats")]
        public async Task<ActionResult<PharmacyDashboardStatsDto>> GetPharmacyStats()
        {
            var stats = await _pharmacistService.GetPharmacyDashboardStatsAsync();
            return Ok(stats);
        }



    }


}
