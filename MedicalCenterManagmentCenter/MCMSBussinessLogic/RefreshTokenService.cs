using MCMSDAL;
using MCMSDAL.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MCMSBLL
{
    public class RefreshTokenResult
    {
        public bool Success { get; set; }
        public bool ReuseDetected { get; set; }
        public Guid UserId { get; set; }
        public string? NewRefreshToken { get; set; }
    }
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenData _refreshTokenData;

        public RefreshTokenService(IRefreshTokenData refreshTokenData)
        {
            _refreshTokenData = refreshTokenData;
        }

        private const int RefreshTokenBytes = 64;
        private const int RefreshTokenDays = 14;

        public async Task<string?> CreateTokenAsync(Guid userId, string? ip, string? userAgent)
        {
            return await CreateInternalAsync(userId, ip, userAgent);
        }

        public async Task<RefreshTokenResult> RotateTokenAsync(string oldRawToken, string? ip, string? userAgent)
        {
            return await RotateInternalAsync(oldRawToken, ip, userAgent);
        }

        public async Task<bool> RevokeTokenAsync(string rawToken, string? ip)
        {
            return await RevokeInternalAsync(rawToken, ip);
        }

        public async Task RevokeAllTokensAsync(Guid userId, string? ip)
        {
            await RevokeAllInternalAsync(userId, ip);
        }

        // =========================================
        // CREATE (called on successful login)
        // =========================================
        private async Task<string?> CreateInternalAsync(
            Guid userId,
            string? ip,
            string? userAgent)
        {
            var rawToken = GenerateSecureToken();
            var hash = HashToken(rawToken);

            var dto = new RefreshTokenDto
            {
                UserId = userId,
                TokenHash = hash,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(RefreshTokenDays),
                CreatedByIp = ip,
                UserAgent = userAgent
            };

            var tokenId = await _refreshTokenData.CreateRefreshTokenAsync(dto);



            if (tokenId == null)
                return null;

            return rawToken; // return RAW token to client
        }

        // =========================================
        // ROTATE (called from refresh endpoint)
        // =========================================
        private async Task<RefreshTokenResult> RotateInternalAsync(
            string oldRawToken,
            string? ip,
            string? userAgent)
        {
            var oldHash = HashToken(oldRawToken);

            var newRawToken = GenerateSecureToken();
            var newHash = HashToken(newRawToken);

            var rotateResult = await _refreshTokenData.RotateAsync(
                oldHash,
                newHash,
                DateTime.UtcNow.AddDays(RefreshTokenDays),
                ip,
                userAgent
            );

            switch (rotateResult.Status)
            {
                case 0: // OK
                    return new RefreshTokenResult
                    {
                        Success = true,
                        UserId = rotateResult.UserId,
                        NewRefreshToken = newRawToken
                    };

                case 3: // Reuse detected
                    return new RefreshTokenResult
                    {
                        Success = false,
                        ReuseDetected = true,
                        UserId = rotateResult.UserId
                    };

                default: // expired / not found
                    return new RefreshTokenResult
                    {
                        Success = false
                    };
            }
        }

        // =========================================
        // LOGOUT SINGLE SESSION
        // =========================================
        private async Task<bool> RevokeInternalAsync(
            string rawToken,
            string? ip)
        {
            var hash = HashToken(rawToken);
            var status = await _refreshTokenData.RevokeAsync(hash, ip);

            return status == 0;
        }

        // =========================================
        // LOGOUT ALL DEVICES
        // =========================================
        private async Task RevokeAllInternalAsync(
            Guid userId,
            string? ip)
        {
            await _refreshTokenData.RevokeAllForUserAsync(userId, ip);
        }

        // =========================================
        // INTERNAL: Secure Token Generator
        // =========================================
        private static string GenerateSecureToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(RefreshTokenBytes);
            return Convert.ToBase64String(bytes);
        }

        // =========================================
        // INTERNAL: SHA256 Hash
        // =========================================
        private static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hashBytes);
        }
    }
}