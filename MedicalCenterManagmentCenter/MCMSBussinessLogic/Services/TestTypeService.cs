using MCMSBussinessLogic.Interfaces;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class TestTypeService : ITestTypeService
    {
        private readonly ITestTypeData _testTypeData;

        public TestTypeService(ITestTypeData testTypeData)
        {
            _testTypeData = testTypeData;
        }

        public async Task<int?> CreateAsync(TestType testType)
        {
            if (await _testTypeData.IsTestTypeExistsByName(testType.Name))
                return null;

            if (testType.Cost < 0)
                throw new InvalidOperationException("Cost cannot be negative");

            var newId = await _testTypeData.CreateTestType(testType.TDTO);
            if (newId <= 0)
                return null;
            testType.TestTypeId = newId;
            return newId;
        }

        public async Task<bool> UpdateAsync(TestType testType)
        {
            if (!await _testTypeData.IsTestTypeExistsById(testType.TestTypeId))
                return false;

            if (testType.Cost < 0)
                throw new InvalidOperationException("Cost cannot be negative");

            return await _testTypeData.UpdateTestType(testType.TDTO);
        }

        public async Task<TestType?> FindByIdAsync(int testTypeId)
        {
            var dto = await _testTypeData.GetTestTypeById(testTypeId);
            return dto != null ? new TestType(dto) : null;
        }

        public async Task<List<TestType>> GetAllAsync()
        {
            var dtos = await _testTypeData.GetAllTestTypes();
            return dtos.Select(dto => new TestType(dto)).ToList();
        }

        public async Task<List<TestType>> GetPagedAsync(int page, int size)
        {
            var dtos = await _testTypeData.GetAllTestTypes();
            return dtos.Skip((page - 1) * size).Take(size).Select(dto => new TestType(dto)).ToList();
        }

        public Task<bool> DeleteAsync(int testTypeId)
        {
            return _testTypeData.DeleteTestType(testTypeId);
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            return _testTypeData.IsTestTypeExistsByName(name);
        }
    }
}
