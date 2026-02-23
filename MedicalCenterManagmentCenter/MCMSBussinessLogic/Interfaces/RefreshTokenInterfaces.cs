namespace MCMSBLL
{
    public interface IRefreshTokenService
    {
        Task<string?> CreateTokenAsync(Guid userId, string? ip, string? userAgent);
        Task<RefreshTokenResult> RotateTokenAsync(string oldRawToken, string? ip, string? userAgent);
        Task<bool> RevokeTokenAsync(string rawToken, string? ip);
        Task RevokeAllTokensAsync(Guid userId, string? ip);
    }
}
