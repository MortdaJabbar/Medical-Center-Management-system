using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace MCMSDAL
{

    public class PatientDoctorDto
    {
        public Guid PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
    }

    public class AppointmentDto
    {
        public int AppointmentID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public  bool  Paid {  get; set; }
        public string Reason { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Notes { get; set; }
    }
    public class AppointmentSummaryDto
    {
        public int AppointmentId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? Reason { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
        public Guid PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public string? PatientImagePath { get; set; }

        public Guid DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string? DoctorImagePath { get; set; }

        public bool Paid { get; set; }
    }
    public class AppointmentPatientDto
    {
        public int AppointmentId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? Reason { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
        public Guid PatientId { get; set; }
      
        public Guid DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string? DoctorImagePath { get; set; }

        public bool Paid { get; set; }
    }
    public class AppointmentByDoctorDto
    {
        public int AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }   // التاريخ فقط
        public TimeOnly AppointmentTime { get; set; }   // الوقت فقط
        public string Status { get; set; }
        public string Notes { get; set; }
        public decimal Cost { get; set; }

        public string PatientFullName { get; set; }
        public string PatientImage { get; set; }
    }



    public static class AppointmentData
    {
        public static async Task<bool> InsertAppointmentAsync(AppointmentDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("InsertAppointment", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PatientID", dto.PatientID);
            command.Parameters.AddWithValue("@DoctorID", dto.DoctorID);
            command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
            command.Parameters.AddWithValue("@Reason", dto.Reason);
            command.Parameters.AddWithValue("@Status", dto.Status);
            command.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@Paid", dto.Paid);
            command.Parameters.AddWithValue("@AppointmentTime", dto.AppointmentTime);

            await connection.OpenAsync();
            int affected = await command.ExecuteNonQueryAsync(); // هذا الرقم يمثل عدد الصفوف


            return affected > 0;
        }

        public static async Task<bool> UpdateAppointmentAsync(AppointmentDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("UpdateAppointment", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@AppointmentID", dto.AppointmentID);
            command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
            command.Parameters.AddWithValue("@Reason", dto.Reason);
            command.Parameters.AddWithValue("@Status", dto.Status);
            command.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@Paid", dto.Paid);
            command.Parameters.AddWithValue("@AppointmentTime", dto.AppointmentTime);
            await connection.OpenAsync();
            int affected = await command.ExecuteNonQueryAsync(); // هذا الرقم يمثل عدد الصفوف


            return affected > 0;
        }

        public static async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetAppointmentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@AppointmentID", id);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new AppointmentDto
                {
                    AppointmentID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    DoctorID = reader.GetGuid(2),
                    AppointmentDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
                    Reason = reader.GetString(4),
                    Status = reader.GetInt32(5),
                    Notes = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Paid = reader.GetBoolean(7),
                    AppointmentTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("AppointmentTime")))

                };
            }

            return null;
        }

        public static async Task<List<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var result = new List<AppointmentDto>();
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetAllAppointments", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new AppointmentDto
                {
                    AppointmentID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    DoctorID = reader.GetGuid(2),
                    AppointmentDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
                    Reason = reader.GetString(4),
                    Status = reader.GetInt32(5),
                    Notes = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Paid = reader.GetBoolean(7),
                    AppointmentTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("AppointmentTime")))
                });
            }

            return result;
        }

        public static async Task<bool> DeleteAppointmentAsync(int id)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("DeleteAppointment", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@AppointmentID", id);

            await connection.OpenAsync();
            int affected = await command.ExecuteNonQueryAsync(); // هذا الرقم يمثل عدد الصفوف


            return affected > 0;
        }

        public static async Task<List<AppointmentPatientDto>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            var result = new List<AppointmentPatientDto>();

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetAppointmentsByPatient", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PatientID", patientId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new AppointmentPatientDto
                {
                    AppointmentId = reader.GetInt32(0),
                    PatientId = reader.GetGuid(1),
                    DoctorId = reader.GetGuid(2),
                    Date = DateOnly.FromDateTime(reader.GetDateTime(3)),
                    Time = TimeOnly.FromTimeSpan(reader.GetTimeSpan(4)),
                    Reason = reader.GetString(5),
                    Status = reader.GetInt32(6),
                    Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
                    Paid = reader.GetBoolean(8),
                    DoctorFullName = reader.GetString(9),
                    DoctorImagePath = reader.IsDBNull(10) ? null : reader.GetString(10)
                });
            }

            return result;
        }

        public static async Task<List<AppointmentByDoctorDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            var appointments = new List<AppointmentByDoctorDto>();

            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("GetAppointmentsByDoctorId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                await con.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        appointments.Add(new AppointmentByDoctorDto
                        {
                            AppointmentId = (int)reader["AppointmentId"],
                            AppointmentDate = DateOnly.FromDateTime((DateTime)reader["AppointmentDate"]),
                            AppointmentTime = TimeOnly.FromTimeSpan((TimeSpan)reader["AppointmentTime"]),
                            Status = reader["Status"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            Cost = (decimal)reader["Cost"],
                            PatientFullName = reader["PatientFullName"].ToString(),
                            PatientImage = reader["PatientImage"].ToString()
                        });
                    }
                }
            }

            return appointments;
        }


        public static async Task<List<AppointmentSummaryDto>> GetAppointmentsWithDetailsAsync()
        {
            var results = new List<AppointmentSummaryDto>();

            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetAllAppointmentsWithDetails", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new AppointmentSummaryDto
                {
                    AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                    Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate"))),
                    Time = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("AppointmentTime"))),
                    Reason = reader["Reason"] as string,
                    Status = reader.GetInt32(reader.GetOrdinal("Status")),
                    Notes =  reader["Notes"].ToString(),
                    PatientId = reader.GetGuid(reader.GetOrdinal("PatientId")),
                    PatientFullName = reader["PatientFullName"].ToString() ?? "",
                    PatientImagePath = reader["PatientImagePath"] as string,

                    DoctorId = reader.GetGuid(reader.GetOrdinal("DoctorId")),
                    DoctorFullName = reader["DoctorFullName"].ToString() ?? "",
                    DoctorImagePath = reader["DoctorImagePath"] as string,
                    Paid = reader.GetBoolean(reader.GetOrdinal("Paid"))

                });
            }

            return results;
        }
    }

}
    
