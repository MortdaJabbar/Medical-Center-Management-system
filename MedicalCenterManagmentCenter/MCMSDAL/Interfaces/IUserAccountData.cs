using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IUserAccountData
    {
        Task<Guid?> CreateUserAccountAsync(UserAccountDto dto);
        Task<bool> ActivateUserAsync(Guid userId);
        Task<UserAccountDto?> FindByEmailAsync(string email);
        Task<List<UserAccountDto>> GetAllAsync();
        Task<UserAccountDto?> FindByIdAsync(Guid userId);
        Task<bool> UpdateAsync(UserAccountDto dto);
        Task<bool> ResetPassword(Guid userId, string newPasswordHash);
        Task<bool> DeleteAsync(Guid userId);
        Task<bool> ChangePasswordAsync(Guid userId, string newPasswordHash);
        Task<List<UserAccountDetailsDto>> GetAllUserAccountsDetailedAsync();
        Task<List<PatientWithoutAccountDto>> GetPatientsWithoutAccountAsync();
        Task<List<DoctorWithoutAccountDto>> GetDoctorsWithoutAccountAsync();
        Task<List<PharmacistWithoutAccountDto>> GetPharmacistsWithoutAccountAsync();
        Task<List<StaffWithoutAccountDto>> GetStaffWithoutAccountAsync();
    }
}
