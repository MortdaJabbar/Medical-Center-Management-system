using AutoMapper;
using MCMSAPI.dtos;
using MCMSBussinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [Authorize(Roles ="Admin")] 
    [Route("api/TestTypes")]
    [ApiController]
 
    public class TestTypesController : ControllerBase
    {
        private readonly IMapper _mapper;

        public TestTypesController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [Authorize(Roles = "Staff")]
        [HttpPost("add")]
        public async Task<IActionResult> AddTestType([FromBody] AddUpdateTestTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors });
            }

            var testType = _mapper.Map<TestType>(dto);
            var success = await testType.AddNewTestTypeAsync();

            return success ? Ok(testType.TestTypeId) : BadRequest("Test type already exists or cost is invalid.");
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTestType(int id, [FromBody] AddUpdateTestTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { errors });
            }

            var existing = await TestType.FindTestTypeByIdAsync(id);
            if (existing == null) return NotFound("Test type not found.");

            _mapper.Map(dto, existing);
            var success = await existing.UpdateTestTypeAsync();

            return success ? Ok("Test type updated.") : BadRequest("Failed to update test type.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestTypeById(int id)
        {
            var testType = await TestType.FindTestTypeByIdAsync(id);
            return testType != null ? Ok(testType.TDTO) : NotFound("Test type not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTestTypes()
        {
            var list = await TestType.GetAllTestTypesAsync();
            var dtoList = list.ConvertAll(t => t.TDTO);
            return Ok(dtoList);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("all/paged")]
        public async Task<IActionResult> GetAllTestTypes(int page = 1, int size = 10)
        {
            var list = await TestType.GetAllTestTypesAsync(page, size);
            var dtoList = list.ConvertAll(t => t.TDTO);
            return Ok(dtoList);
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestType(int id)
        {
            var deleted = await TestType.DeleteTestTypeByIdAsync(id);
            return deleted ? Ok("Test type deleted.") : NotFound("Test type not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("exists/by-name/{name}")]
        public async Task<IActionResult> CheckExistsByName(string name)
        {
            bool exists = await TestType.IsTestTypeExistsByNameAsync(name);
            return Ok(new { exists });
        }
    }

}
