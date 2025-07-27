using AutoMapper;
using MCMSAPI.dtos.PharmacistDto;
using MCMSBussinessLogic;
using MCMSDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace MCMSAPI.Controllers
{
    [Route("api/Pharmacists")]
    [ApiController]
    public class PharmacistController : ControllerBase
    {
        private readonly IMapper _mapper;

        public PharmacistController(IMapper mapper)
        {
            _mapper = mapper;
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
            bool isAdded = await pharmacist.AddNewPharmacistAsync();

            return isAdded ? Ok(pharmacist.DTO.PharmacistId) : BadRequest("Pharmacist already exists or cannot be added.");
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

            Pharmacist? pharmacist = await Pharmacist.FindPharmacistByIdAsync(id);
            if (pharmacist == null) return NotFound("Pharmacist not found.");

            _mapper.Map(pharmacistDto, pharmacist);
            bool updated = await pharmacist.UpdatePharmacistAsync();

            return updated ? Ok("Updated successfully.") : NotFound("Pharmacist could not be updated.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacistById(Guid id)
        {
            var pharmacist = await Pharmacist.FindPharmacistByIdAsync(id);
            return pharmacist != null ? Ok(pharmacist.DTO) : NotFound("Pharmacist not found.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPharmacists( )
        {
            var pharmacists = await Pharmacist.GetAllPharmacistsAsync( );
            var dtoList = pharmacists.ConvertAll(p => p.DTO);
            return Ok(dtoList);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{pharmacistId}")]
        public async Task<IActionResult> DeletePharmacist(Guid pharmacistId)
        {
            Pharmacist Pharmist = await Pharmacist.FindPharmacistByIdAsync(pharmacistId);
            if (pharmacistId == null) { return NotFound("Pharmacist not found"); }
            bool deleted = await Pharmacist.DeletePharmacistByIdAsync(pharmacistId, Pharmist.PersonId);
            return deleted ? Ok("Deleted successfully.") : StatusCode(500, "Internal Error in our servers  ");
        }
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("pharmacy-stats")]
        public async Task<ActionResult<PharmacyDashboardStatsDto>> GetPharmacyStats()
        {
            var stats = await Pharmacist.GetPharmacyDashboardStatsAsync();
            return Ok(stats);
        }



    }


}
