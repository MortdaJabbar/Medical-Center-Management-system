using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IStaffData
    {
        Task<Guid> InsertStaffAsync(StaffDTO staff);
        Task<bool> UpdateStaffAsync(Guid staffId, StaffDTO staff);
        Task<bool> DeleteStaffAsync(Guid staffId);
        Task<StaffDTO?> GetStaffByIdAsync(Guid staffId);
        Task<List<StaffDTO>> GetAllStaffAsync();
        Task<List<StaffSummaryDto>> GetAllStaffSummariesAsync();
        Task<StaffDashboardStatsDto> GetStaffDashboardStatsAsync();
        Task<AdminDashboardStatsDto> GetAdminDashboardStatsAsync();
    }
}
