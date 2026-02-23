using MCMSAPI.Helper;
using MCMSDAL;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;


namespace MCMSBussinessLogic
{
    public class UserAccount : IUserAccount
    {
        private static readonly UserAccountData _userAccountData = new();

        [JsonIgnore]
      
        public UserAccountDto DTO => new UserAccountDto
        {
            UserId = UserId,
            PersonId = PersonId,
            Email = Email,
            PasswordHash = PasswordHash,
            RoleId = RoleId,
            IsActive = IsActive,
            Is2FAEnabled = Is2FAEnabled,
           
        };

        public Guid UserId { get; set; }
        public Guid PersonId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public bool Is2FAEnabled { get; set; }
        public string RoleText { 
            get 
            {
                switch (RoleId) 
                {
                    case 1: return "Admin";
                    case 2: return "Doctor";
                    case 3: return "Patient";
                    case 4: return "Pharmacist";
                    case 5: return "Staff";
                    default:return "Unknown";
                }
                
            } 
        
        }

        public UserAccount() { }

        public UserAccount(UserAccountDto dto)
        {
            UserId = dto.UserId;
            PersonId = dto.PersonId;
            Email = dto.Email;
            PasswordHash = dto.PasswordHash;
            RoleId = dto.RoleId;
            IsActive = dto.IsActive;
            Is2FAEnabled = dto.Is2FAEnabled;
            
        }


        public async Task<bool> RegisterAsync()
        {
            var userId = await _userAccountData.CreateUserAccountAsync(this.DTO);

            if (userId == null)
                return false;

            UserId = userId.Value;
            return true;
        }

        public async Task<bool> ActivateUserAsync() 
        {
            return await _userAccountData.ActivateUserAsync(this.UserId);
        
        }

        public static async Task<UserAccount?> FindByEmailAsync(string Email)
        {
            UserAccountDto? dto = await _userAccountData.FindByEmailAsync(Email);

            return (dto != null) ? new UserAccount(dto) : null;

        }
        public static async Task<UserAccount?> FindByIDAsync(Guid UserId)
        {
            UserAccountDto? dto = await _userAccountData.FindByIdAsync(UserId);

            return (dto != null) ? new UserAccount(dto) : null;

        }
        public static async Task<bool> DeleteAccountAsync(Guid UserId) 
        {
            return await _userAccountData.DeleteAsync(UserId);
        }
        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (!PasswordHelper.VerifyPassword(currentPassword, this.PasswordHash))
                return false;

            this.PasswordHash = PasswordHelper.HashPassword(newPassword);
            return await _userAccountData.ChangePasswordAsync(this.UserId, this.PasswordHash);
        }
        public async Task<bool> UpdateAsync()
        {
            return await _userAccountData.UpdateAsync(this.DTO);
        }

        public async static  Task<List<UserAccount>> GetAllAsync() 
        {
                    var dtos =   await _userAccountData.GetAllAsync();
            return dtos.Select(dto => new UserAccount(dto)).ToList();

        }

        public static  async Task<List<UserAccountDetailsDto>> GetAllUserAccountsAsync()
        {
            // يمكنك إضافة أي منطق أعمال هنا مستقبلاً مثل التحقق أو التصنيف
            return await _userAccountData.GetAllUserAccountsDetailedAsync();
        }

        public static async Task<List<PatientWithoutAccountDto>> GetPatientsWithoutAccountAsync()
        {
            return await _userAccountData.GetPatientsWithoutAccountAsync();
        }

        public static async Task<List<DoctorWithoutAccountDto>> GetDoctorsWithoutAccountAsync()
        {
            return await _userAccountData.GetDoctorsWithoutAccountAsync();
        }

        public static async Task<List<PharmacistWithoutAccountDto>> GetPharmacistsWithoutAccountAsync()
        {
            return await _userAccountData.GetPharmacistsWithoutAccountAsync();
        }

        public static async Task<List<StaffWithoutAccountDto>> GetStaffWithoutAccountAsync()
        {
            return await _userAccountData.GetStaffWithoutAccountAsync();
        }

       public async Task<bool> ResetPassword() 
        {

            return await _userAccountData.ResetPassword(this.UserId, this.PasswordHash);
        
        }



    }



}
