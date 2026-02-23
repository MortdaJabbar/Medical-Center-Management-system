using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCMSDAL.Interfaces;

namespace MCMSDAL
{

    
    public class EmailVerificationData : IEmailVerificationData
    {
        public async Task<bool> CreateVerificationAsync(Guid userId, string token, DateTime expiry)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("CreateEmailVerification", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.Parameters.AddWithValue("@ExpiryDate", expiry);

            await conn.OpenAsync();
            return (await cmd.ExecuteNonQueryAsync())>0;
        }
        public async Task<(Guid UserId, bool IsUsed, DateTime ExpiryDate)?> FindByTokenAsync(string token)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
        SELECT UserId, IsUsed, ExpiryDate 
        FROM EmailVerifications 
        WHERE Token = @Token", conn);

            cmd.Parameters.AddWithValue("@Token", token);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    reader.GetGuid(0),
                    reader.GetBoolean(1),
                    reader.GetDateTime(2)
                );
            }

            return null;
        }
        public async Task MarkAsUsedAsync(string token)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
        UPDATE EmailVerifications 
        SET IsUsed = 1 
        WHERE Token = @Token", conn);

            cmd.Parameters.AddWithValue("@Token", token);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

          




    }

}
