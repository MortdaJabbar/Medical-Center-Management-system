using Microsoft.Data.SqlClient;
using System.Data;
using MCMSDAL.Interfaces;

namespace MCMSDAL
{
    public class RefreshTokenDto
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? RevokedAtUtc { get; set; }
        public Guid? ReplacedByTokenId { get; set; }

        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public string? UserAgent { get; set; }
    }

    public class RefreshTokenRotateResult
    {
        public int Status { get; set; }              // 0 ok, 1 not found, 2 expired, 3 reuse
        public Guid UserId { get; set; }
        public Guid NewTokenId { get; set; }
    }

    public class RefreshTokenData : IRefreshTokenData
    {
        public async Task<Guid?> CreateRefreshTokenAsync(RefreshTokenDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("CreateRefreshToken", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", dto.UserId);
            command.Parameters.AddWithValue("@TokenHash", dto.TokenHash);
            command.Parameters.AddWithValue("@ExpiresAtUtc", dto.ExpiresAtUtc.ToUniversalTime());
            command.Parameters.AddWithValue("@CreatedByIp", (object?)dto.CreatedByIp ?? DBNull.Value);
            command.Parameters.AddWithValue("@UserAgent", (object?)dto.UserAgent ?? DBNull.Value);

            var tokenIdParam = new SqlParameter("@TokenId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(tokenIdParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return tokenIdParam.Value != DBNull.Value ? (Guid?)tokenIdParam.Value : null;
        }

        public async Task<RefreshTokenDto?> FindByHashAsync(string tokenHash)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetRefreshTokenByHash", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TokenHash", tokenHash);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new RefreshTokenDto
            {
                TokenId = reader.GetGuid(reader.GetOrdinal("TokenId")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                TokenHash = reader.GetString(reader.GetOrdinal("TokenHash")),
                CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("CreatedAtUtc")),
                ExpiresAtUtc = reader.GetDateTime(reader.GetOrdinal("ExpiresAtUtc")),
                RevokedAtUtc = reader.IsDBNull(reader.GetOrdinal("RevokedAtUtc")) ? null : reader.GetDateTime(reader.GetOrdinal("RevokedAtUtc")),
                ReplacedByTokenId = reader.IsDBNull(reader.GetOrdinal("ReplacedByTokenId")) ? null : reader.GetGuid(reader.GetOrdinal("ReplacedByTokenId")),
                CreatedByIp = reader.IsDBNull(reader.GetOrdinal("CreatedByIp")) ? null : reader.GetString(reader.GetOrdinal("CreatedByIp")),
                RevokedByIp = reader.IsDBNull(reader.GetOrdinal("RevokedByIp")) ? null : reader.GetString(reader.GetOrdinal("RevokedByIp")),
                UserAgent = reader.IsDBNull(reader.GetOrdinal("UserAgent")) ? null : reader.GetString(reader.GetOrdinal("UserAgent")),
            };
        }

        public async Task<RefreshTokenRotateResult> RotateAsync(
            string oldTokenHash,
            string newTokenHash,
            DateTime newExpiresAtUtc,
            string? requestIp,
            string? userAgent)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("RotateRefreshToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OldTokenHash", oldTokenHash);
            cmd.Parameters.AddWithValue("@NewTokenHash", newTokenHash);
            cmd.Parameters.AddWithValue("@NewExpiresAtUtc", newExpiresAtUtc.ToUniversalTime());
            cmd.Parameters.AddWithValue("@RequestIp", (object?)requestIp ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);

            var newTokenIdParam = new SqlParameter("@NewTokenId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };
            var userIdParam = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };
            var statusParam = new SqlParameter("@Status", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(newTokenIdParam);
            cmd.Parameters.Add(userIdParam);
            cmd.Parameters.Add(statusParam);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return new RefreshTokenRotateResult
            {
                Status = statusParam.Value == DBNull.Value ? -1 : (int)statusParam.Value,
                UserId = userIdParam.Value == DBNull.Value ? Guid.Empty : (Guid)userIdParam.Value,
                NewTokenId = newTokenIdParam.Value == DBNull.Value ? Guid.Empty : (Guid)newTokenIdParam.Value
            };
        }

        public async Task<int> RevokeAsync(string tokenHash, string? requestIp)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("RevokeRefreshToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TokenHash", tokenHash);
            cmd.Parameters.AddWithValue("@RequestIp", (object?)requestIp ?? DBNull.Value);

            var statusParam = new SqlParameter("@Status", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(statusParam);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return statusParam.Value == DBNull.Value ? -1 : (int)statusParam.Value;
        }

        public async Task<bool> RevokeAllForUserAsync(Guid userId, string? requestIp)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("RevokeAllRefreshTokensForUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RequestIp", (object?)requestIp ?? DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
    }
}