using MCMSAPI.Helper;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountData _userAccountData;

        public UserAccountService(IUserAccountData userAccountData)
        {
            _userAccountData = userAccountData;
        }

        public async Task<List<UserAccount>> GetAllAsync()
        {
            var dtos = await _userAccountData.GetAllAsync();
            return dtos.Select(dto => new UserAccount(dto)).ToList();
        }

        public async Task<UserAccount?> FindByIdAsync(Guid userId)
        {
            var dto = await _userAccountData.FindByIdAsync(userId);
            return dto != null ? new UserAccount(dto) : null;
        }

        public async Task<UserAccount?> FindByEmailAsync(string email)
        {
            var dto = await _userAccountData.FindByEmailAsync(email);
            return dto != null ? new UserAccount(dto) : null;
        }

        public Task<bool> DeleteAsync(Guid userId)
        {
            return _userAccountData.DeleteAsync(userId);
        }

        public Task<bool> UpdateAsync(UserAccount user)
        {
            return _userAccountData.UpdateAsync(user.DTO);
        }

        public async Task<bool> ChangePasswordAsync(UserAccount user, string currentPassword, string newPassword)
        {
            if (!PasswordHelper.VerifyPassword(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            return await _userAccountData.ChangePasswordAsync(user.UserId, user.PasswordHash);
        }

        public Task<List<UserAccountDetailsDto>> GetAllUserAccountsDetailedAsync()
        {
            return _userAccountData.GetAllUserAccountsDetailedAsync();
        }

        public Task<List<PatientWithoutAccountDto>> GetPatientsWithoutAccountAsync()
        {
            return _userAccountData.GetPatientsWithoutAccountAsync();
        }

        public Task<List<DoctorWithoutAccountDto>> GetDoctorsWithoutAccountAsync()
        {
            return _userAccountData.GetDoctorsWithoutAccountAsync();
        }

        public Task<List<PharmacistWithoutAccountDto>> GetPharmacistsWithoutAccountAsync()
        {
            return _userAccountData.GetPharmacistsWithoutAccountAsync();
        }

        public Task<List<StaffWithoutAccountDto>> GetStaffWithoutAccountAsync()
        {
            return _userAccountData.GetStaffWithoutAccountAsync();
        }
    }
}
