using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface ITestTypeData
    {
        Task<int> CreateTestType(TestTypeDto testType);
        Task<TestTypeDto> GetTestTypeById(int testTypeId);
        Task<bool> UpdateTestType(TestTypeDto testType);
        Task<bool> DeleteTestType(int testTypeId);
        Task<bool> IsTestTypeExistsById(int testTypeId);
        Task<bool> IsTestTypeExistsByName(string name);
        Task<List<TestTypeDto>> GetAllTestTypes();
    }
}
