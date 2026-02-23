using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IDoctorData
    {
        Task<Guid> AddNewDoctorAsync(DoctorDTO doctor);
        Task<DoctorDTO> GetDoctorByIdAsync(Guid doctorId);
        Task<List<DoctorDTO>> GetAllDoctorsAsync();
        Task<bool> UpdateDoctorAsync(DoctorDTO doctor);
        Task<bool> DeleteDoctorAsync(Guid doctorId);
        Task<bool> IsDoctorExistsByIdAsync(Guid doctorId);
        Task<bool> IsDoctorExistsByNameAsync(string firstName, string secondName, string? thirdName = null);
        Task<bool> IsDoctorExistsByPersonIdAsync(Guid personId);
        Task<DoctorDashboardStatsDto?> GetDoctorDashboardStatsAsync(Guid doctorId);
    }
}
