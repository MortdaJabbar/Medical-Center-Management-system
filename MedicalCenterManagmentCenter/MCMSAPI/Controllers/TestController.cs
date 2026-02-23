namespace MCMSAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using AutoMapper;
    using MCMSAPI.dtos;
    using MCMSBussinessLogic;
    using MCMSBussinessLogic.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using MCMSDAL;

    [Route("api/Tests")]
    [ApiController]
    
    public class TestsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITestService _testService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public TestsController(IMapper mapper, ITestService testService, IDoctorService doctorService, IPatientService patientService)
        {
            _mapper = mapper;
            _testService = testService;
            _doctorService = doctorService;
            _patientService = patientService;
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
            var newId = await _testService.CreateAsync(test);

            return newId != null ? Ok(newId.Value) : BadRequest("Test could not be added.");
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

            Test? test = await _testService.FindByIdAsync(id);
            if (test == null) return NotFound("Test not found.");
            test.Cost = UpdatedTest.Cost;
            test.TestResult = UpdatedTest.TestResult;
            test.Status = UpdatedTest.Stauts;
            test.Notes = UpdatedTest.Notes;


            var updated = await _testService.UpdateAsync(test);

            return updated ? Ok("Updated successfully.") : NotFound("Test not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestById(int id)
        {
            var test = await _testService.FindByIdAsync(id);
            return test != null ? Ok(_mapper.Map<TestDto>(test)) : NotFound("Test not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests(int page = 1, int size = 10)
        {
            var tests = await _testService.GetPagedAsync(page, size);
           
            return Ok(tests);
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            bool deleted = await _testService.DeleteAsync(id);
            return deleted ? Ok("Deleted successfully.") : NotFound("Test not found.");
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailedTests()
        {
            var result = await _testService.GetAllDetailedAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetTestsByDoctorId(
    Guid doctorId,
    [FromServices] IAuthorizationService authorizationService)
        {
            if (doctorId == Guid.Empty)
                return BadRequest("Invalid Doctor ID");

            var doctor = await _doctorService.FindByIdAsync(doctorId);

            if (doctor == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                doctor.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();

            var tests = await _testService.GetByDoctorIdAsync(doctorId);

            return Ok(tests);
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetTestsByPatientId(Guid patientId,
    [FromServices] IAuthorizationService authorizationService)
        {
            if (patientId == Guid.Empty)
                return BadRequest("Invalid Patient ID");

            var patient = await _patientService.FindByIdAsync(patientId);

            if (patient == null)
                return NotFound();

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                patient.PersonId,
                "OwnerOnly");

            if (!authResult.Succeeded)
                return Forbid();
            var result = await _testService.GetByPatientIdAsync(patientId);
            return Ok(result);
            
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("pairs")]
        public async Task<IActionResult> GetPairs()
        {
            var result = await _testService.GetPairsAsync();
            return Ok(result);
        }


    }

}
