using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IPatientData
    {
        Task<Guid> CreatePatientAsync(PatientDTO patient);
        Task<PatientDTO> GetPatientByIdAsync(Guid patientId);
        Task<List<PatientDTO>> GetAllPatientsAsync();
        Task<bool> UpdatePatientAsync(PatientDTO patient);
        Task<bool> DeletePatientAsync(Guid patientId);
        Task<bool> IsPatientExistsByIdAsync(Guid patientId);
        Task<bool> IsPatientExistsByNameAsync(string firstName, string secondName, string? thirdName = null);
        Task<bool> IsPatientExistsByPersonIdAsync(Guid personId);
        Task<PatientDashboardDto?> GetPatientDashboardStatsAsync(Guid patientId);
    }
}
