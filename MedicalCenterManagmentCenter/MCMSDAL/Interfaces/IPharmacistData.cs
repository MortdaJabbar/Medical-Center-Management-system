using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IPharmacistData
    {
        Task<Guid> CreatePharmacistAsync(PharmacistDTO pharmacist);
        Task<PharmacistDTO?> GetPharmacistByIdAsync(Guid pharmacistId);
        Task<bool> UpdatePharmacistAsync(PharmacistDTO pharmacist);
        Task<bool> DeletePharmacistAsync(Guid pharmacistId);
        Task<bool> IsPharmacistExistsByIdAsync(Guid pharmacistId);
        Task<bool> IsPharmacistExistsByNameAsync(string firstName, string secondName, string? thirdName = null);
        Task<bool> IsPharmacistExistsByPersonIdAsync(Guid personId);
        Task<List<PharmacistDTO>> GetAllPharmacistsAsync();
        Task<PharmacyDashboardStatsDto> GetPharmacyDashboardStatsAsync();
    }
}
