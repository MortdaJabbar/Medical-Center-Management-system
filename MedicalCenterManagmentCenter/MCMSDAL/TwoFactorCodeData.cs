using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{

        public static class TwoFactorCodeData
        {
            public static async Task<bool> CreateCodeAsync(Guid userId, string code, DateTime expiry)
            {
                using var conn = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("CreateTwoFactorCode", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Code", code);
                cmd.Parameters.AddWithValue("@Expiry", expiry);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }

            public static async Task<(string Code, DateTime Expiry, bool IsUsed)?> GetLatestCodeAsync(Guid userId)
            {
                using var conn = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand(@"
            SELECT TOP 1 Code, Expiry, IsUsed
            FROM TwoFactorCodes
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC", conn);

                cmd.Parameters.AddWithValue("@UserId", userId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return (
                        reader.GetString(0),
                        reader.GetDateTime(1),
                        reader.GetBoolean(2)
                    );
                }

                return null;
            }

            public static async Task<bool> MarkAsUsedAsync(Guid userId, string code)
            {
                using var conn = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand(@"
            UPDATE TwoFactorCodes
            SET IsUsed = 1
            WHERE UserId = @UserId AND Code = @Code", conn);

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Code", code);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

    }

