using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;


namespace MCMSDAL
{
    public class PatientWithoutAccountDto
    {
        public Guid PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class DoctorWithoutAccountDto
    {
        public Guid DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class PharmacistWithoutAccountDto
    {
        public Guid PharmacistId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class StaffWithoutAccountDto
    {
        public Guid StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class RegisterUserDto
    {
        [Required]
        public Guid PersonId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(10, ErrorMessage = "Password must be at least 10 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; } // Admin = 1, Doctor = 2, Patient = 3, etc.

    }

    public class UserAccountDetailsDto
    {
        public Guid UserId { get; set; }
        public Guid PersonId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool Is2FAEnabled { get; set; }
        public string UserImage { get; set; } = string.Empty; // مسار الصورة
    }

    public class UserAccountDto
    {
        public Guid UserId { get; set; }
        public Guid PersonId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public bool Is2FAEnabled { get; set; }
        
        
    }

    public class UserAccountData
    {
        // Method to create a new user account
        public static async Task<Guid?> CreateUserAccountAsync(UserAccountDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("CreateUserAccount", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parameters
            command.Parameters.AddWithValue("@PersonId", dto.PersonId);
            command.Parameters.AddWithValue("@Email", dto.Email);
            command.Parameters.AddWithValue("@PasswordHash", dto.PasswordHash);
            command.Parameters.AddWithValue("@RoleId", dto.RoleId);
            

            // Output parameter for UserId
            var userIdParam = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(userIdParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return userIdParam.Value != DBNull.Value
                ? (Guid?)userIdParam.Value
                : null;
        }
        public static async Task<bool> ActivateUserAsync(Guid userId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("ActivateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int affectedRows = reader.GetInt32(0); // SELECT @@ROWCOUNT
                return affectedRows > 0;
            }

            return false;
        }
        public static async Task<UserAccountDto?> FindByEmailAsync(string email)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetUserAccountByEmail", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Email", email);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserAccountDto
                {
                    UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                    PersonId = reader.GetGuid(reader.GetOrdinal("PersonId")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    Is2FAEnabled = reader.GetBoolean(reader.GetOrdinal("Is2FAEnabled"))
                };
            }

            return null;
        }
        public static async Task<List<UserAccountDto>> GetAllAsync()
        {
         
            var list = new List<UserAccountDto>();
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetAllUserAccounts", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new UserAccountDto
                {
                    UserId = reader.GetGuid(0),
                    PersonId = reader.GetGuid(1),
                    Email = reader.GetString(2),
                    RoleId = reader.GetInt32(3),
                    IsActive = reader.GetBoolean(4),
                    Is2FAEnabled = reader.GetBoolean(5),
                    PasswordHash = ""
                });
            }

            return list;
        }
        public static async Task<UserAccountDto?> FindByIdAsync(Guid userId)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetUserAccountById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", userId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync()
                ? new UserAccountDto
                {
                    UserId = reader.GetGuid(0),
                    PersonId = reader.GetGuid(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    RoleId = reader.GetInt32(4),
                    IsActive = reader.GetBoolean(5),
                    Is2FAEnabled = reader.GetBoolean(6)
                }
                : null;
        }
        public static async Task<bool> UpdateAsync(UserAccountDto dto)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("UpdateUserAccount", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", dto.UserId);
            cmd.Parameters.AddWithValue("@Email", dto.Email);
            cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
            cmd.Parameters.AddWithValue("@2FA", dto.Is2FAEnabled);

            await conn.OpenAsync();
            int rowsAffected = (int)await cmd.ExecuteScalarAsync();
            return  rowsAffected > 0;
        }

        public static async Task<bool> ResetPassword(Guid userId, string newPasswordHash)
        {
            using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        UPDATE UserAccounts
        SET PasswordHash = @NewPasswordHash
        WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return  (rows > 0);
            }
        }

        public static async Task<bool> DeleteAsync(Guid userId)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("DeleteUserAccount", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", userId);
            await conn.OpenAsync();
            int rowsAffected = (int)await cmd.ExecuteScalarAsync();
            return rowsAffected > 0;
        }
        public static async Task<bool> ChangePasswordAsync(Guid userId, string newPasswordHash)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("ChangeUserPassword", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int rows = reader.GetInt32(0); // يقرأ RowAffected
                return rows > 0;
            }
            return false;
        }
        public static async Task<List<UserAccountDetailsDto>> GetAllUserAccountsDetailedAsync()
        {
            var users = new List<UserAccountDetailsDto>();

            using (SqlConnection connection = new SqlConnection(AppConfig.ConnectionString))
            using (SqlCommand command = new SqlCommand("GetAllUserAccountsDetailed", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new UserAccountDetailsDto
                        {
                            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                            PersonId = reader.GetGuid(reader.GetOrdinal("PersonId")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Role = reader.GetString(reader.GetOrdinal("Role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            Is2FAEnabled = reader.GetBoolean(reader.GetOrdinal("Is2FAEnabled")),
                            UserImage = reader.IsDBNull(reader.GetOrdinal("UserImage"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("UserImage"))
                        };

                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public static async Task<List<PatientWithoutAccountDto>> GetPatientsWithoutAccountAsync()
        {
            var list = new List<PatientWithoutAccountDto>();

            using SqlConnection connection = new SqlConnection(AppConfig.ConnectionString);
            using SqlCommand command = new SqlCommand("GetPatientsWithoutAccount", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PatientWithoutAccountDto
                {
                    PatientId = reader.GetGuid(reader.GetOrdinal("PatientId")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName"))
                });
            }

            return list;
        }
        public static async Task<List<DoctorWithoutAccountDto>> GetDoctorsWithoutAccountAsync()
        {
            var list = new List<DoctorWithoutAccountDto>();

            using SqlConnection connection = new SqlConnection(AppConfig.ConnectionString);
            using SqlCommand command = new SqlCommand("GetDoctorsWithoutAccount", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DoctorWithoutAccountDto
                {
                    DoctorId = reader.GetGuid(reader.GetOrdinal("DoctorId")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName"))
                });
            }

            return list;
        }
        public static async Task<List<PharmacistWithoutAccountDto>> GetPharmacistsWithoutAccountAsync()
        {
            var list = new List<PharmacistWithoutAccountDto>();

            using SqlConnection connection = new SqlConnection(AppConfig.ConnectionString);
            using SqlCommand command = new SqlCommand("GetPharmacistsWithoutAccount", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PharmacistWithoutAccountDto
                {
                    PharmacistId = reader.GetGuid(reader.GetOrdinal("PharmacistId")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName"))
                });
            }

            return list;
        }
        public static async Task<List<StaffWithoutAccountDto>> GetStaffWithoutAccountAsync()
        {
            var list = new List<StaffWithoutAccountDto>();

            using SqlConnection connection = new SqlConnection(AppConfig.ConnectionString);
            using SqlCommand command = new SqlCommand("GetStaffWithoutAccount", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new StaffWithoutAccountDto
                {
                    StaffId = reader.GetGuid(reader.GetOrdinal("StaffId")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName"))
                });
            }

            return list;
        }





    }


}


