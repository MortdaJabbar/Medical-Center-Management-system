using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface ITestData
    {
        Task<int> CreateTestAsync(TestDto test);
        Task<TestDto?> GetTestByIdAsync(int testId);
        Task<bool> UpdateTestAsync(TestDto test);
        Task<bool> DeleteTestAsync(int testId);
        Task<bool> IsTestExistsByIdAsync(int testId);
        Task<List<TestDetailsDto>> GetAllTestsAsync();
        Task<List<TestDoctorDto>> GetTestsByDoctorIdAsync(Guid doctorId);
        Task<List<TestPatientsDto>> GetTestsByPatientIdAsync(Guid patientId);
        Task<List<PatientDoctorDto>> GetPatientDoctorPairsAsync();
    }
}
