using Microsoft.Data.SqlClient;
using System.Data;


namespace MCMSDAL
{
    public class PatientSummaryDto
    {
        public Guid PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }
    public class PatientDashboardDto
    {
        public int TotalTests { get; set; }
        public int TotalPrescriptions { get; set; }
        public int UpcomingAppointments { get; set; }
        public DateOnly? LastTestDate { get; set; }
        public string LastTestStatus { get; set; } = "Unknown";
    }


    public class PatientDTO
    {
        public Guid PatientId { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public PersonDTO Person { get; set; }   
    }

    public class PatientData
    {
        public static async Task<Guid> CreatePatientAsync(PatientDTO patient)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.InsertPatient", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PersonId", patient.Person.PersonId);
            command.Parameters.AddWithValue("@Weight", patient.Weight);
            command.Parameters.AddWithValue("@Height", patient.Height);

            
            await connection.OpenAsync();

            var PersonId = await command.ExecuteScalarAsync();

            return (Guid)PersonId;
        }

        public static async Task<PatientDTO> GetPatientByIdAsync(Guid patientId)
        {
            PatientDTO? patient = null;

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.GetPatientById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PatientId", patientId);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
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

                        patient = new PatientDTO
                        {
                            PatientId = reader.GetGuid(reader.GetOrdinal("PatientId")),
                            Weight = reader.GetDecimal(reader.GetOrdinal("Weight")),
                            Height = reader.GetDecimal(reader.GetOrdinal("Height")),
                            Person = person
                        };
                    }
                }
            }

            return patient;
        }

        public static async Task<List<PatientDTO>> GetAllPatientsAsync( )
        {
            var patients = new List<PatientDTO>();

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.GetAllPatients", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
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

                        var patient = new PatientDTO
                        {
                            PatientId = reader.GetGuid(reader.GetOrdinal("PatientId")),
                            Weight = reader.GetDecimal(reader.GetOrdinal("Weight")),
                            Height = reader.GetDecimal(reader.GetOrdinal("Height")),
                            Person = person
                        };

                        patients.Add(patient);
                    }
                }
            }

            return patients;
        }

        public static async Task<bool> UpdatePatientAsync(PatientDTO patient)
        {
            if (!await IsPatientExistsByIdAsync(patient.PatientId)) return false;

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.UpdatePatient", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PatientId", patient.PatientId);
                command.Parameters.AddWithValue("@PersonId", patient.Person.PersonId);
                command.Parameters.AddWithValue("@Weight", patient.Weight);
                command.Parameters.AddWithValue("@Height", patient.Height);

                await connection.OpenAsync();
                int rowsAffected = (int)await command.ExecuteScalarAsync();
                return (rowsAffected > 0);
            }
        }

        public static async Task<bool> DeletePatientAsync(Guid patientId)
        {
            if (!await IsPatientExistsByIdAsync(patientId)) return false;

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.DeletePatient", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PatientId", patientId);

                await connection.OpenAsync();
                int rowsAffected = (int)await command.ExecuteScalarAsync();
                return (rowsAffected > 0);
            }
        }

        public static async Task<bool> IsPatientExistsByIdAsync(Guid patientId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.IsPatientExistsById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PatientId", patientId);

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToBoolean(result);
            }
        }
        public static async Task<bool> IsPatientExistsByNameAsync(string firstName, string secondName, string? thirdName = null)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsPatientExistsByName", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", (object?)thirdName ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }
        public static async Task<bool> IsPatientExistsByPersonIdAsync(Guid personId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsPatientExistsByPersonId", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PersonId", personId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }

        public static async Task<PatientDashboardDto?> GetPatientDashboardStatsAsync(Guid patientId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetPatientDashboardStats", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PatientID", patientId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new PatientDashboardDto
                {
                    TotalTests = Convert.ToInt32(reader["TotalTests"]),
                    TotalPrescriptions = Convert.ToInt32(reader["TotalPrescriptions"]),
                    UpcomingAppointments = Convert.ToInt32(reader["UpcomingAppointments"]),
                    LastTestDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("LastTestDate"))),
                    LastTestStatus = reader["LastTestStatus"].ToString() ?? "Unknown"
                };
            }

            return null;
        }


    }

}
