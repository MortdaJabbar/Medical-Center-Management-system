using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCMSDAL;
using System.ComponentModel.DataAnnotations;
namespace MCMSDAL
{

    public class StaffDashboardStatsDto
    {
        public decimal TotalPayments { get; set; }
        public int UnpaidServices { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalTests { get; set; }
    }
    public class AdminDashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalPatients { get; set; }
        public int TotalPharmacists { get; set; }
        public int TotalStaff { get; set; }
        public int TotalAccounts { get; set; }
    }

    public class AddUpdatePersonDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Second name is required.")]
        public string SecondName { get; set; }

        public string? ThirdName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateOnly DateOfBirth { get; set; }
        public bool Gender { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address Form.")]
        public string Email { get; set; }

        public string? ImageLocation { get; set; }
    }
    public class StaffSummaryDto
    {
        public Guid StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateOnly HireDate { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class StaffDTO
    {
        public Guid StaffId { get; set; }
        public DateOnly HireDate { get; set; }
        public bool IsAdmin { get; set; }
        public PersonDTO Person { get; set; } = null!;
    }

   

    public class AddUpdateStaffDto
    {
        public AddUpdatePersonDto Person { get; set; }
        public bool IsAdmin { get; set; }
        public DateOnly HireDate { get; set; }

    }

    public static class StaffData
    {
        public static async Task<Guid> InsertStaffAsync(StaffDTO staff)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.InsertStaff", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PersonId", staff.Person.PersonId);
            command.Parameters.AddWithValue("@IsAdmin", staff.IsAdmin);
            command.Parameters.AddWithValue("@HireDate", staff.HireDate);

            await connection.OpenAsync();
            return (Guid)(await command.ExecuteScalarAsync());
        }

        public static async Task<bool> UpdateStaffAsync(Guid staffId, StaffDTO staff)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.UpdateStaff", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@StaffId", staffId);
            command.Parameters.AddWithValue("@IsAdmin", staff.IsAdmin);
            command.Parameters.AddWithValue("@HireDate", staff.HireDate);

            await connection.OpenAsync();
            return (int)(await command.ExecuteScalarAsync()) > 0;
        }

        public static async Task<bool> DeleteStaffAsync(Guid staffId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.DeleteStaff", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@StaffId", staffId);

            await connection.OpenAsync();
            return (int)(await command.ExecuteScalarAsync()) > 0;
        }

        public static async Task<StaffDTO?> GetStaffByIdAsync(Guid staffId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetStaffById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@StaffId", staffId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var person = new PersonDTO(
                    reader.GetGuid(reader.GetOrdinal("PersonId")),
                    reader.GetString(reader.GetOrdinal("FirstName")),
                    reader.GetString(reader.GetOrdinal("SecondName")),
                    reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? null : reader.GetString(reader.GetOrdinal("ThirdName")),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))),
                    reader.GetBoolean(reader.GetOrdinal("Gender")),
                    reader.GetString(reader.GetOrdinal("Phone")),
                    reader.GetString(reader.GetOrdinal("Email")),
                    reader.IsDBNull(reader.GetOrdinal("ImageLocation")) ? null : reader.GetString(reader.GetOrdinal("ImageLocation"))
                );

                return new StaffDTO
                {
                    StaffId = staffId,
                    HireDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("HireDate"))),
                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin")),
                    Person = person
                };
            }

            return null;
        }

        public static async Task<List<StaffDTO>> GetAllStaffAsync()
        {
            var list = new List<StaffDTO>();
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetAllStaff", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var person = new PersonDTO(
                    reader.GetGuid(reader.GetOrdinal("PersonId")),
                    reader.GetString(reader.GetOrdinal("FirstName")),
                    reader.GetString(reader.GetOrdinal("SecondName")),
                    reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? null : reader.GetString(reader.GetOrdinal("ThirdName")),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))),
                    reader.GetBoolean(reader.GetOrdinal("Gender")),
                    reader.GetString(reader.GetOrdinal("Phone")),
                    reader.GetString(reader.GetOrdinal("Email")),
                    reader.IsDBNull(reader.GetOrdinal("ImageLocation")) ? null : reader.GetString(reader.GetOrdinal("ImageLocation"))
                );

                list.Add(new StaffDTO
                {
                    StaffId = reader.GetGuid(reader.GetOrdinal("StaffId")),
                    HireDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("HireDate"))),
                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin")),
                    Person = person
                });
            }

            return list;
        }

        public static async Task<List<StaffSummaryDto>> GetAllStaffSummariesAsync()
        {
            var list = new List<StaffSummaryDto>();

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetAllStaffSummaries", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new StaffSummaryDto
                {
                    StaffId = reader.GetGuid(reader.GetOrdinal("StaffId")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    ImagePath = reader.IsDBNull(reader.GetOrdinal("ImageLocation"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("ImageLocation")),
                    HireDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("HireDate"))),
                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
                });
            }

            return list;
        }

        public static async Task<StaffDashboardStatsDto> GetStaffDashboardStatsAsync()
        {
            var dto = new StaffDashboardStatsDto();

            using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("GetStaffDashboardStats", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        dto.TotalPayments = reader.GetDecimal(0);
                        dto.UnpaidServices = reader.GetInt32(1);
                        dto.TotalAppointments = reader.GetInt32(2);
                        dto.TotalTests = reader.GetInt32(3);
                    }
                }
            }

            return dto;
        }

        public static async Task<AdminDashboardStatsDto> GetAdminDashboardStatsAsync()
        {
            var dto = new AdminDashboardStatsDto();

            using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("GetAdminDashboardStats", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        dto.TotalUsers = reader.GetInt32(0);
                        dto.TotalPatients = reader.GetInt32(1);
                        dto.TotalPharmacists = reader.GetInt32(2);
                        dto.TotalStaff = reader.GetInt32(3);
                        dto.TotalAccounts = reader.GetInt32(4);
                    }
                }
            }

            return dto;
        }



    }



}
