using Microsoft.Data.SqlClient;
using System.Data;

namespace MCMSDAL
{
    public class PharmacistSummaryDto
    {
        public Guid PharmacistId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }
    public class PharmacistDTO
    {
        public Guid PharmacistId { get; set; }
        public string LicenseNumber { get; set; }
        public int? ExpereinceYears { get; set; }
        public DateOnly HireDate { get; set; }
        public PersonDTO Person { get; set; }
    }
    public class PharmacyDashboardStatsDto
    {
        public int TotalMedications { get; set; }
        public int TotalPrescriptions { get; set; }
        public int InventoryStock { get; set; }
        public int LowStockCount { get; set; }
    }

    public class PharmacistData
    {
        public static async Task<Guid> CreatePharmacistAsync(PharmacistDTO pharmacist)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.InsertPharmacist", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PersonId", pharmacist.Person.PersonId);
            command.Parameters.AddWithValue("@LicenseNumber", pharmacist.LicenseNumber);
            command.Parameters.AddWithValue("@HireDate", pharmacist.HireDate);
            command.Parameters.AddWithValue("@ExpereinceYears", pharmacist.ExpereinceYears);

            await connection.OpenAsync();
            var pharmacistId = await command.ExecuteScalarAsync();
            return (Guid)pharmacistId;
        }
        public static async Task<PharmacistDTO?> GetPharmacistByIdAsync(Guid pharmacistId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetPharmacistById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PharmacistId", pharmacistId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                var person = new PersonDTO(
                    reader.GetGuid(reader.GetOrdinal("PersonId")),
                    reader.GetString(reader.GetOrdinal("FirstName")),
                    reader.GetString(reader.GetOrdinal("SecondName")),
                    reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? null : reader.GetString(reader.GetOrdinal("ThirdName")),
                   DateOnly.FromDateTime((reader.GetDateTime(reader.GetOrdinal("DateOfBirth")))),
                    reader.GetBoolean(reader.GetOrdinal("Gender")),
                    reader.GetString(reader.GetOrdinal("Phone")),
                    reader.GetString(reader.GetOrdinal("Email")),
                    reader.IsDBNull(reader.GetOrdinal("ImageLocation")) ? null : reader.GetString(reader.GetOrdinal("ImageLocation"))
                );

                return new PharmacistDTO
                {
                    PharmacistId = reader.GetGuid(reader.GetOrdinal("PharmacistID")),
                   
                    LicenseNumber = reader.GetString(reader.GetOrdinal("LicenseNumber")),
                    HireDate = DateOnly.FromDateTime((reader.GetDateTime(reader.GetOrdinal("HireDate")))),
                    ExpereinceYears = reader.GetInt32(reader.GetOrdinal("ExpereinceYears")),
                    Person = person
                };
            }

            return null;
        }
        public static async Task<bool> UpdatePharmacistAsync(PharmacistDTO pharmacist)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.UpdatePharmacist", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PharmacistId", pharmacist.PharmacistId);
            command.Parameters.AddWithValue("@LicenseNumber", pharmacist.LicenseNumber);
            command.Parameters.AddWithValue("@HireDate", pharmacist.HireDate);
            command.Parameters.AddWithValue("@ExpereinceYears", pharmacist.ExpereinceYears);

            await connection.OpenAsync();
            var result =  (int) await command.ExecuteScalarAsync();
            return result > 0;
        }
        public static async Task<bool> DeletePharmacistAsync(Guid pharmacistId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.DeletePharmacist", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PharmacistID", pharmacistId);

            await connection.OpenAsync();
            var result = (int)await command.ExecuteScalarAsync();
            return result > 0;
        }
        public static async Task<bool> IsPharmacistExistsByIdAsync(Guid pharmacistId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsPharmacistExistsById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PharmacistId", pharmacistId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }
        public static async Task<bool> IsPharmacistExistsByNameAsync(string firstName, string secondName, string? thirdName = null)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsPharmacistExistsByName", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", (object?)thirdName ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }
        public static async Task<bool> IsPharmacistExistsByPersonIdAsync(Guid personId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsPharmacistExistsByPersonId", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PersonId", personId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }
        public static async Task<List<PharmacistDTO>> GetAllPharmacistsAsync( )
        {
            var pharmacists = new List<PharmacistDTO>();

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetAllPharmacists", connection);
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
                      DateOnly.FromDateTime((reader.GetDateTime( reader.GetOrdinal("DateOfBirth")) )),
                    reader.GetBoolean(reader.GetOrdinal("Gender")),
                    reader.GetString(reader.GetOrdinal("Phone")),
                    reader.GetString(reader.GetOrdinal("Email")),
                    reader.IsDBNull(reader.GetOrdinal("ImageLocation")) ? null : reader.GetString(reader.GetOrdinal("ImageLocation"))
                );

                var pharmacist = new PharmacistDTO
                {
                    PharmacistId    = reader.GetGuid(reader.GetOrdinal("PharmacistId")),
                    LicenseNumber   = reader.GetString(reader.GetOrdinal("LicenseNumber")),
                    HireDate        =  DateOnly.FromDateTime((reader.GetDateTime(reader.GetOrdinal("HireDate")))),
                    ExpereinceYears = reader.GetInt32(reader.GetOrdinal("ExpereinceYears")),
                    Person = person
                };

                pharmacists.Add(pharmacist);
            }

            return pharmacists;
        }

        
            public static async Task<PharmacyDashboardStatsDto> GetPharmacyDashboardStatsAsync()
            {
                var dto = new PharmacyDashboardStatsDto();

                using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand("GetPharmacyDashboardStats", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            dto.TotalMedications = reader.GetInt32(0);
                            dto.TotalPrescriptions = reader.GetInt32(1);
                            dto.InventoryStock = reader.GetInt32(2);
                            dto.LowStockCount = reader.GetInt32(3);
                        }
                    }
                }

                return dto;
            }
        
        



    }


}
