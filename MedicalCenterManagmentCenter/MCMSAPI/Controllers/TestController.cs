namespace MCMSAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using AutoMapper;
    using MCMSAPI.dtos;
    using MCMSBussinessLogic;
    using MCMSDAL;
    using Microsoft.AspNetCore.Authorization;

    [Route("api/Tests")]
    [ApiController]
    
    public class TestsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public TestsController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [Authorize(Roles = "Staff")]
        [HttpPost("add")]
        public async Task<IActionResult> AddTest([FromBody] AddUpdateTestDto addTestDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { errors });
            }

            var test = _mapper.Map<Test>(addTestDto);
            bool isAdded = await test.AddNewTestAsync();

            return isAdded ? Ok(test.TestID) : BadRequest("Test could not be added.");
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] UpdateTestDto UpdatedTest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { errors });
            }

            Test? test = await Test.FindTestByIdAsync(id);
            test.Cost = UpdatedTest.Cost;
            test.TestResult = UpdatedTest.TestResult;
            test.Status = UpdatedTest.Stauts;
            test.Notes = UpdatedTest.Notes;



            var updated = await test.UpdateTestAsync();

            return updated ? Ok("Updated successfully.") : NotFound("Test not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestById(int id)
        {
            var test = await Test.FindTestByIdAsync(id);
            return test != null ? Ok(_mapper.Map<TestDto>(test)) : NotFound("Test not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests(int page = 1, int size = 10)
        {
            var tests = await Test.GetAllTestsAsync(page, size);
           
            return Ok(tests);
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            bool deleted = await Test.DeleteTestAsync(id);
            return deleted ? Ok("Deleted successfully.") : NotFound("Test not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailedTests()
        {
            var result = await Test.GetAllTestsAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetTestsByDoctorId(Guid doctorId)
        {
            var tests = await Test.GetTestsByDoctorIdAsync(doctorId);
            

            return Ok(tests);

        }
        [Authorize(Roles = "Patient")]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetTestsByPatientId(Guid patientId)
        {
            var result = await Test.GetTestsByPatientIdAsync(patientId);
            return Ok(result);
            
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("pairs")]
        public async Task<IActionResult> GetPairs()
        {
            var result = await Test.GetAllPatientDoctorPairsAsync();
            return Ok(result);
        }


    }

}
