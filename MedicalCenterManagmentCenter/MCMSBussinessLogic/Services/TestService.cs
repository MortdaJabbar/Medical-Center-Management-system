using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class TestService : ITestService
    {
        private readonly ITestData _testData;

        public TestService(ITestData testData)
        {
            _testData = testData;
        }

        public async Task<int?> CreateAsync(Test test)
        {
            if (test.Cost < 0)
                throw new InvalidOperationException("Cost cannot be negative");

            test.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

            var newId = await _testData.CreateTestAsync(test.TDTO);
            if (newId <= 0)
                return null;
            test.TestID = newId;
            return newId;
        }

        public async Task<bool> UpdateAsync(Test test)
        {
            if (!await _testData.IsTestExistsByIdAsync(test.TestID))
                return false;

            return await _testData.UpdateTestAsync(test.TDTO);
        }

        public async Task<Test?> FindByIdAsync(int testId)
        {
            var dto = await _testData.GetTestByIdAsync(testId);
            return dto != null ? new Test(dto) : null;
        }

        public async Task<List<TestDetailsDto>> GetPagedAsync(int page, int size)
        {
            var all = await _testData.GetAllTestsAsync();
            return all.Skip((page - 1) * size).Take(size).ToList();
        }

        public Task<List<TestDetailsDto>> GetAllDetailedAsync()
        {
            return _testData.GetAllTestsAsync();
        }

        public Task<bool> DeleteAsync(int testId)
        {
            return _testData.DeleteTestAsync(testId);
        }

        public Task<List<TestDoctorDto>> GetByDoctorIdAsync(Guid doctorId)
        {
            return _testData.GetTestsByDoctorIdAsync(doctorId);
        }

        public Task<List<TestPatientsDto>> GetByPatientIdAsync(Guid patientId)
        {
            return _testData.GetTestsByPatientIdAsync(patientId);
        }

        public Task<List<PatientDoctorDto>> GetPairsAsync()
        {
            return _testData.GetPatientDoctorPairsAsync();
        }
    }
}
