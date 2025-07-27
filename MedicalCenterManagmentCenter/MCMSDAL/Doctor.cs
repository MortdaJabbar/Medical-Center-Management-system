using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{
    public class DoctorSummaryDto
{
    public Guid DoctorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}

    public class DoctorDashboardStatsDto
    {
        public int TotalPatients { get; set; }
        public int UpcomingAppointments { get; set; }
        public int TotalPrescriptions { get; set; }
        public int TotalTests { get; set; }
    }

    public class DoctorDTO
    {
        public Guid DoctorId { get; set; }
        public string Specialization { get; set; }
        public bool Available { get; set; }
        public int? ScheduleId { get; set; } // Nullable int
        public int Experienceyears { get; set; } // Nullable int
        public PersonDTO Person { get; set; }
    }

    public class DoctorData
    {
        public static async Task<Guid> AddNewDoctorAsync(DoctorDTO doctor)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.InsertDoctor", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PersonId", doctor.Person.PersonId);
            command.Parameters.AddWithValue("@Specialization", doctor.Specialization);
            command.Parameters.AddWithValue("@Available", doctor.Available);
            command.Parameters.AddWithValue("@ScheduleId", (object?)doctor.ScheduleId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Experienceyears", (object?)doctor.Experienceyears ?? DBNull.Value);

            await connection.OpenAsync();
            var doctorId = await command.ExecuteScalarAsync();
            return (Guid)doctorId;
        }

        public static async Task<DoctorDTO> GetDoctorByIdAsync(Guid doctorId)
        {
            DoctorDTO? doctor = null;

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetDoctorById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (reader.Read())
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

                doctor = new DoctorDTO
                {
                    DoctorId = reader.GetGuid(reader.GetOrdinal("DoctorId")),
                    Specialization = reader.GetString(reader.GetOrdinal("Specialization")),
                    Available = reader.GetBoolean(reader.GetOrdinal("Available")),
                    ScheduleId = reader.IsDBNull(reader.GetOrdinal("ScheduleId"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("ScheduleId")),
                    Experienceyears = reader.GetInt32(reader.GetOrdinal("Experienceyears")),
                    Person = person
                };
            }

            return doctor;
        }

        public static async Task<List<DoctorDTO>> GetAllDoctorsAsync()
        {
            var doctors = new List<DoctorDTO>();

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetAllDoctors", connection);
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

                var doctor = new DoctorDTO
                {
                    DoctorId = reader.GetGuid(reader.GetOrdinal("DoctorId")),
                    Specialization = reader.GetString(reader.GetOrdinal("Specialization")),
                    Available = reader.GetBoolean(reader.GetOrdinal("Available")),
                    ScheduleId = reader.IsDBNull(reader.GetOrdinal("ScheduleId"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("ScheduleId")),
                    Experienceyears =  reader.GetInt32(reader.GetOrdinal("Experienceyears")),
                    Person = person
                };

                doctors.Add(doctor);
            }

            return doctors;
        }

        public static async Task<bool> UpdateDoctorAsync(DoctorDTO doctor)
        {
            if (!await IsDoctorExistsByIdAsync(doctor.DoctorId)) return false;

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.UpdateDoctor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DoctorId", doctor.DoctorId);
            command.Parameters.AddWithValue("@Specialization", doctor.Specialization);
            command.Parameters.AddWithValue("@Available", doctor.Available);
            command.Parameters.AddWithValue("@Experienceyears", doctor.Experienceyears);
            command.Parameters.AddWithValue("@ScheduleId", (object?)doctor.ScheduleId ?? DBNull.Value);

            await connection.OpenAsync();
            int rowsAffected = (int)await command.ExecuteScalarAsync();
            return rowsAffected > 0;
        }

        public static async Task<bool> DeleteDoctorAsync(Guid doctorId)
        {
           

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.DeleteDoctor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DoctorID", doctorId);

            await connection.OpenAsync();
            int rowsAffected = (int)await command.ExecuteScalarAsync();
            return rowsAffected > 0;
        }

        public static async Task<bool> IsDoctorExistsByIdAsync(Guid doctorId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsDoctorExistsById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }
        public static async Task<bool> IsDoctorExistsByNameAsync(string firstName, string secondName, string? thirdName = null)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsDoctorExistsByName", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", (object?)thirdName ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }
        public static async Task<bool> IsDoctorExistsByPersonIdAsync(Guid personId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsDoctorExistsByPersonId", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PersonId", personId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }

        public static  async Task<DoctorDashboardStatsDto?> GetDoctorDashboardStatsAsync(Guid doctorId)
        {
            using SqlConnection con = new SqlConnection(AppConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("GetDoctorDashboardStats", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new DoctorDashboardStatsDto
                {
                    TotalPatients = (int)reader["TotalPatients"],
                    UpcomingAppointments = (int)reader["UpcomingAppointments"],
                    TotalPrescriptions = (int)reader["TotalPrescriptions"],
                    TotalTests = (int)reader["TotalTests"]
                };
            }

            return null;
        }




    }





}
