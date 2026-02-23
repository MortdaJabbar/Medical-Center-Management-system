using MCMSDAL;

namespace MCMSBussinessLogic
{
    public class Test : ITest
    {
        private static readonly TestData _testData = new();

        public TestDto TDTO
        {
            get => new TestDto
            {
                TestID = TestID,
                PatientID = PatientID,
                DoctorID = DoctorID,
                TestTypeID = TestTypeID,
                CreatedAt = CreatedAt,
                Status = Status,
                Notes = Notes,
                Cost = Cost,
                TestResult = TestResult
            };
        }
        public int TestID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public int TestTypeID { get; set; }
        public DateOnly CreatedAt { get; set; }
        public int Status { get; set; }
        public string StatusText 
        { get 
            {
                switch (Status)
                {
                    case 1: return "Pending";
                    case 2: return "Canceld";
                    case 3: return "Completed";
                    default:return "Unknown";
                }    
                  
                    } 
            }
        public string? Notes { get; set; }
        public decimal Cost { get; set; }
        public string? TestResult { get; set; }
        public Test()
        {
        }
        public Test(TestDto testDto)
        {
            if (testDto == null)
                throw new ArgumentNullException(nameof(testDto));

            TestID = testDto.TestID;
            PatientID = testDto.PatientID;
            DoctorID = testDto.DoctorID;
            TestTypeID = testDto.TestTypeID;
            CreatedAt = testDto.CreatedAt;
            Status = testDto.Status;
            Notes = testDto.Notes;
            Cost = testDto.Cost;
            TestResult = testDto.TestResult;
        }

        public async Task<bool> AddNewTestAsync()
        {
            if (Cost < 0)
                throw new InvalidOperationException("Cost cannot be negative");

        
            CreatedAt = DateOnly.FromDateTime(DateTime.Now);

            int result = await _testData.CreateTestAsync(TDTO);
            this.TestID = result;
            return result > 0;
        }
        public static async Task<Test?> FindTestByIdAsync(int testId)
        {
            var testDto = await _testData.GetTestByIdAsync(testId);
            return testDto != null ? new Test(testDto) : null;
        }
        public async Task<bool> UpdateTestAsync()
        {
            if (!await _testData.IsTestExistsByIdAsync(TestID))
                return false;

            return await _testData.UpdateTestAsync(TDTO);
        }
        public static async Task<bool> DeleteTestAsync(int testId)
        {
            return await _testData.DeleteTestAsync(testId);
        }
        public static async Task<bool> IsTestExistsByIdAsync(int testId)
        {
            return await _testData.IsTestExistsByIdAsync(testId);
        }
        public static async Task<List<TestDetailsDto>> GetAllTestsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var testDtos = await _testData.GetAllTestsAsync();

            var paginatedTests = testDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return paginatedTests;
        }
        public static async Task<List<TestDetailsDto>> GetAllTestsAsync()
        {
            var testDtos = await _testData.GetAllTestsAsync();

            
            return testDtos;
        }
        public static async Task<List<TestPatientsDto>> GetTestsByPatientIdAsync(Guid patientId)
        {
            var allTests = await _testData.GetTestsByPatientIdAsync(patientId);
            

            return allTests;
        }
        public static async Task<List<TestDoctorDto>> GetTestsByDoctorIdAsync(Guid DoctorId)
        {
            var Tests = await _testData.GetTestsByDoctorIdAsync(DoctorId);
           

            return Tests;
        }

        public static async Task<List<PatientDoctorDto>> GetAllPatientDoctorPairsAsync()
        {
            return await _testData.GetPatientDoctorPairsAsync();
        }


    }
}
