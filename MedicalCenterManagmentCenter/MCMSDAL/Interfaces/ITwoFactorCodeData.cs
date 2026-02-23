using System;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface ITwoFactorCodeData
    {
        Task<bool> CreateCodeAsync(Guid userId, string code, DateTime expiry);
        Task<(string Code, DateTime Expiry, bool IsUsed)?> GetLatestCodeAsync(Guid userId);
        Task<bool> MarkAsUsedAsync(Guid userId, string code);
    }
}
