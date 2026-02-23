using System;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IEmailVerificationData
    {
        Task<bool> CreateVerificationAsync(Guid userId, string token, DateTime expiry);
        Task<(Guid UserId, bool IsUsed, DateTime ExpiryDate)?> FindByTokenAsync(string token);
        Task MarkAsUsedAsync(string token);
    }
}
