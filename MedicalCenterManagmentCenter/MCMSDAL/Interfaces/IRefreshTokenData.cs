using System;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IRefreshTokenData
    {
        Task<Guid?> CreateRefreshTokenAsync(RefreshTokenDto dto);
        Task<RefreshTokenDto?> FindByHashAsync(string tokenHash);
        Task<RefreshTokenRotateResult> RotateAsync(string oldTokenHash, string newTokenHash, DateTime newExpiresAtUtc, string? requestIp, string? userAgent);
        Task<int> RevokeAsync(string tokenHash, string? requestIp);
        Task<bool> RevokeAllForUserAsync(Guid userId, string? requestIp);
    }
}
