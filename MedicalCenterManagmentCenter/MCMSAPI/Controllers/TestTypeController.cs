using AutoMapper;
using MCMSAPI.dtos;
using MCMSBussinessLogic;
using MCMSBussinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [Authorize(Roles ="Staff")] 
    [Route("api/TestTypes")]
    [ApiController]
 
    public class TestTypesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITestTypeService _testTypeService;

        public TestTypesController(IMapper mapper, ITestTypeService testTypeService)
        {
            _mapper = mapper;
            _testTypeService = testTypeService;
        }
      
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
            var newId = await _testTypeService.CreateAsync(testType);

            return newId != null ? Ok(newId.Value) : BadRequest("Test type already exists or cost is invalid.");
        }
         
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

            var existing = await _testTypeService.FindByIdAsync(id);
            if (existing == null) return NotFound("Test type not found.");

            _mapper.Map(dto, existing);
            var success = await _testTypeService.UpdateAsync(existing);

            return success ? Ok("Test type updated.") : BadRequest("Failed to update test type.");
        }
      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestTypeById(int id)
        {
            var testType = await _testTypeService.FindByIdAsync(id);
            return testType != null ? Ok(testType.TDTO) : NotFound("Test type not found.");
        }
         
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTestTypes()
        {
            var list = await _testTypeService.GetAllAsync();
            var dtoList = list.ConvertAll(t => t.TDTO);
            return Ok(dtoList);
        }
        
        [HttpGet("all/paged")]
        public async Task<IActionResult> GetAllTestTypes(int page = 1, int size = 10)
        {
            var list = await _testTypeService.GetPagedAsync(page, size);
            var dtoList = list.ConvertAll(t => t.TDTO);
            return Ok(dtoList);
        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestType(int id)
        {
            var deleted = await _testTypeService.DeleteAsync(id);
            return deleted ? Ok("Test type deleted.") : NotFound("Test type not found.");
        }
       
        [HttpGet("exists/by-name/{name}")]
        public async Task<IActionResult> CheckExistsByName(string name)
        {
            bool exists = await _testTypeService.ExistsByNameAsync(name);
            return Ok(new { exists });
        }
    }

}
