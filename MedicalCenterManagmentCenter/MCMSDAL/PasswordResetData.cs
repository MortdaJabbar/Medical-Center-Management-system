using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{
    public class RestPasswordTokenDto 
    {
       public  Guid userId { get; set; } 
        public string token { get; set; }
       public DateTime expiry { get; set; }
    }
    public static class PasswordResetData
    {
        public static async Task CreateResetTokenAsync(RestPasswordTokenDto dto)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("CreatePasswordResetToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Token", dto.token);
            cmd.Parameters.AddWithValue("@UserId", dto.userId);
            cmd.Parameters.AddWithValue("@Expiry", dto.expiry);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<RestPasswordTokenDto?> GetByTokenAsync(string token)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetPasswordResetToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Token", token);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (new RestPasswordTokenDto
                {
                    userId = reader.GetGuid(reader.GetOrdinal("UserId")),
                    expiry = reader.GetDateTime(reader.GetOrdinal("Expiry")),
                    token = token
                }
                );
            }

            return null;
        }

        public static async Task DeleteTokenAsync(string token)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("DeletePasswordResetToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Token", token);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }

}
