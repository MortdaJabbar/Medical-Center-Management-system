using System.Security.Cryptography;
using System.Text;
using MCMSDAL;

namespace MCMSBussinessLogic
{
    public class RefreshTokenIssueResult
    {
        public string RefreshToken { get; set; } = "";
        public string RefreshTokenHash { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }
    }

    public class RefreshTokenValidateResult
    {
        public bool IsValid { get; set; }
        public bool IsReuseDetected { get; set; }
        public Guid UserId { get; set; }
        public string? Error { get; set; }
    }

    public static class RefreshTokenService
    {
        // Generate raw refresh token (what client stores)
        public static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Base64UrlEncode(bytes);
        }

        // Hash token for storage (CSV/DB)
        public static string HashToken(string token, string hashKey)
        {
            // HMAC-SHA256 so attacker can’t precompute hashes without hashKey
            var keyBytes = Encoding.UTF8.GetBytes(hashKey);
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(tokenBytes);
            return Convert.ToHexString(hash); // hex string
        }

        public static async Task<RefreshTokenIssueResult> IssueAsync(
            Guid userId,
            string csvPath,
            string hashKey,
            int expiresInDays,
            string? ip,
            string? userAgent)
        {
            var refresh = GenerateRefreshToken();
            var hash = HashToken(refresh, hashKey);

            var rec = new RefreshTokenRecord
            {
                TokenId = Guid.NewGuid(),
                UserId = userId,
                TokenHash = hash,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(expiresInDays),
                RevokedAtUtc = null,
                ReplacedByTokenHash = null,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByIp = ip,
                UserAgent = userAgent
            };

            await RefreshTokenCsvData.AddAsync(csvPath, rec);

            return new RefreshTokenIssueResult
            {
                RefreshToken = refresh,
                RefreshTokenHash = hash,
                ExpiresAtUtc = rec.ExpiresAtUtc
            };
        }

        public static async Task<RefreshTokenValidateResult> ValidateForRotationAsync(
            string refreshToken,
            string csvPath,
            string hashKey)
        {
            var hash = HashToken(refreshToken, hashKey);
            var all = await RefreshTokenCsvData.GetAllAsync(csvPath);

            var rec = all.FirstOrDefault(x => x.TokenHash == hash);
            if (rec == null)
                return new RefreshTokenValidateResult { IsValid = false, Error = "Unknown refresh token." };

            if (rec.ExpiresAtUtc < DateTime.UtcNow)
                return new RefreshTokenValidateResult { IsValid = false, UserId = rec.UserId, Error = "Refresh token expired." };

            // If revoked already => reuse attempt (very important)
            if (rec.RevokedAtUtc != null)
                return new RefreshTokenValidateResult
                {
                    IsValid = false,
                    IsReuseDetected = true,
                    UserId = rec.UserId,
                    Error = "Refresh token reuse detected."
                };

            return new RefreshTokenValidateResult
            {
                IsValid = true,
                UserId = rec.UserId
            };
        }

        public static async Task RevokeAndReplaceAsync(
            string oldRefreshToken,
            string newRefreshTokenHash,
            string csvPath,
            string hashKey)
        {
            var oldHash = HashToken(oldRefreshToken, hashKey);

            await RefreshTokenCsvData.UpdateAsync(
                csvPath,
                r => r.TokenHash == oldHash,
                r =>
                {
                    r.RevokedAtUtc = DateTime.UtcNow;
                    r.ReplacedByTokenHash = newRefreshTokenHash;
                });
        }

        public static async Task RevokeAllForUserAsync(Guid userId, string csvPath)
        {
            await RefreshTokenCsvData.UpdateAsync(
                csvPath,
                r => r.UserId == userId && r.RevokedAtUtc == null,
                r => r.RevokedAtUtc = DateTime.UtcNow);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
    }
}