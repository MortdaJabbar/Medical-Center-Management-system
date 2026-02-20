using System.Security.Cryptography;
using MCMSDAL; // where RefreshTokenFileData + RefreshTokenRecord lives

namespace MCMSBLL.Auth
{
    public class RefreshTokenService
    {
        private readonly TimeSpan _refreshLifetime;

        public RefreshTokenService(TimeSpan refreshLifetime)
        {
            _refreshLifetime = refreshLifetime;
        }

        public async Task<(string refreshToken, DateTime expiresAtUtc)> IssueAsync(
            Guid userId,
            string? ip,
            string? userAgent)
        {
            var now = DateTime.UtcNow;

            var rawToken = GenerateSecureToken();
            var hash = RefreshTokenFileData.HashToken(rawToken);

            var rec = new RefreshTokenRecord
            {
                UserId = userId,
                TokenHash = hash,
                CreatedAtUtc = now,
                ExpiresAtUtc = now.Add(_refreshLifetime),
                CreatedByIp = ip,
                UserAgent = userAgent
            };

            await RefreshTokenFileData.AddAsync(rec);
            return (rawToken, rec.ExpiresAtUtc);
        }

        // Validate + Rotate
        public async Task<(bool ok, Guid userId, string newRefreshToken, DateTime newRefreshExpiresAtUtc, string? error)>
            RefreshAsync(string incomingRefreshToken, string? ip, string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(incomingRefreshToken))
                return (false, Guid.Empty, "", default, "Missing refresh token");

            var now = DateTime.UtcNow;

            var oldHash = RefreshTokenFileData.HashToken(incomingRefreshToken);
            var existing = await RefreshTokenFileData.FindByHashAsync(oldHash);

            if (existing == null)
                return (false, Guid.Empty, "", default, "Invalid refresh token");

            if (RefreshTokenFileData.IsRevoked(existing))
                return (false, Guid.Empty, "", default, "Refresh token revoked");

            if (RefreshTokenFileData.IsExpired(existing, now))
                return (false, Guid.Empty, "", default, "Refresh token expired");

            // ROTATION: create new one
            var (newRaw, newExp) = await IssueAsync(existing.UserId, ip, userAgent);
            var newHash = RefreshTokenFileData.HashToken(newRaw);

            // revoke old and link to new
            var revoked = await RefreshTokenFileData.RevokeAsync(
                tokenHash: oldHash,
                revokedAtUtc: now,
                revokedByIp: ip,
                replacedByTokenHash: newHash
            );

            if (!revoked)
                return (false, Guid.Empty, "", default, "Failed to rotate refresh token");

            return (true, existing.UserId, newRaw, newExp, null);
        }

        public async Task<bool> RevokeAsync(string refreshToken, string? ip)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var now = DateTime.UtcNow;
            var hash = RefreshTokenFileData.HashToken(refreshToken);

            return await RefreshTokenFileData.RevokeAsync(hash, now, ip, replacedByTokenHash: null);
        }

        // ---- helpers ----
        private static string GenerateSecureToken()
        {
            // 64 bytes -> base64 string ~ 88 chars
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}