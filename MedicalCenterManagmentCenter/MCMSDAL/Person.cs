
using System.Data;
using Microsoft.Data.SqlClient;

namespace MCMSDAL
{
    public class PersonProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string ImageLocation { get; set; } = string.Empty;
        public string CurrentRole { get; set; } = string.Empty;
    }

    public class PersonDTO
    {
        public PersonDTO() { }
        public PersonDTO(Guid personId, string firstName, string secondName, string? thirdName, DateOnly dateOfBirth, bool gender, string phone, string email, string? imageLocation)
        {
            PersonId = personId;
            FirstName = firstName;
            SecondName = secondName;
            ThirdName = thirdName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Phone = phone;
            Email = email;
            ImageLocation = imageLocation;
        }


        public Guid PersonId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? ThirdName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? ImageLocation { get; set; }
    }

    public class PersonData
    {
        public static async Task<List<PersonDTO>> GetAllPeopleAsync(int PageNumber = 1 , int PageSize = 10 )
        {
            var personList = new List<PersonDTO>();

            using (SqlConnection conn = new SqlConnection(AppConfig.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.GetAllPeople", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personList.Add(new PersonDTO(
                            reader.GetGuid(reader.GetOrdinal("PersonId")),
                            reader.GetString(reader.GetOrdinal("FirstName")),
                            reader.GetString(reader.GetOrdinal("SecondName")),
                            reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? null : reader.GetString(reader.GetOrdinal("ThirdName")),
                            reader.GetFieldValue<DateOnly>(reader.GetOrdinal("DateOfBirth")),
                            reader.GetBoolean(reader.GetOrdinal("Gender")),
                            reader.GetString(reader.GetOrdinal("Phone")),
                            reader.GetString(reader.GetOrdinal("Email")),
                            reader.IsDBNull(reader.GetOrdinal("ImageLocation")) ? null : reader.GetString(reader.GetOrdinal("ImageLocation"))
                        ));
                    }
                }
            }

            return personList;
        }
        public static async Task<PersonDTO?> GetPersonByIdAsync(Guid personId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.GetPersonById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PersonId", personId);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new PersonDTO(
                            reader.GetGuid(reader.GetOrdinal("PersonId")),
                            reader.GetString(reader.GetOrdinal("FirstName")),
                            reader.GetString(reader.GetOrdinal("SecondName")),
                            reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? null : reader.GetString(reader.GetOrdinal("ThirdName")),
                            reader.GetFieldValue<DateOnly>(reader.GetOrdinal("DateOfBirth")),
                            reader.GetBoolean(reader.GetOrdinal("Gender")),
                            reader.GetString(reader.GetOrdinal("Phone")),
                            reader.GetString(reader.GetOrdinal("Email")),
                            reader.IsDBNull(reader.GetOrdinal("ImageLocation")) ? null : reader.GetString(reader.GetOrdinal("ImageLocation"))
                        );
                    }
                }
            }

            return null;
        }
        public static async Task<Guid> AddPersonAsync(PersonDTO PDTO)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.InsertPerson", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FirstName", PDTO.FirstName);
                command.Parameters.AddWithValue("@SecondName", PDTO.SecondName);
                command.Parameters.AddWithValue("@ThirdName", (object?)PDTO.ThirdName ?? DBNull.Value);
                command.Parameters.AddWithValue("@DateOfBirth", PDTO.DateOfBirth);
                command.Parameters.AddWithValue("@Gender", PDTO.Gender);
                command.Parameters.AddWithValue("@Phone", PDTO.Phone);
                command.Parameters.AddWithValue("@Email", PDTO.Email);
                command.Parameters.AddWithValue("@ImageLocation", (object?)PDTO.ImageLocation ?? DBNull.Value);

               
                
                await connection.OpenAsync();
               var PersonId=  await command.ExecuteScalarAsync();

                return (Guid)PersonId;
            }
        }
        public static async Task<bool> UpdatePersonAsync(PersonDTO PDTO)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.UpdatePerson", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PersonId", PDTO.PersonId);
                command.Parameters.AddWithValue("@FirstName", PDTO.FirstName);
                command.Parameters.AddWithValue("@SecondName", PDTO.SecondName);
                command.Parameters.AddWithValue("@ThirdName", (object?)PDTO.ThirdName ?? DBNull.Value);
                command.Parameters.AddWithValue("@DateOfBirth", PDTO.DateOfBirth);
                command.Parameters.AddWithValue("@Gender", PDTO.Gender);
                command.Parameters.AddWithValue("@Phone", PDTO.Phone);
                command.Parameters.AddWithValue("@Email", PDTO.Email);
                command.Parameters.AddWithValue("@ImageLocation", (object?)PDTO.ImageLocation ?? DBNull.Value);

                await connection.OpenAsync();
                int rowsAffected = (int ) await command.ExecuteScalarAsync();
                return (rowsAffected > 0);
            }
        }
        public static async Task<bool> DeletePersonAsync(Guid personId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.DeletePerson", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PersonId", personId);

                await connection.OpenAsync();
                int rowsAffected = (int)await command.ExecuteScalarAsync();
                return rowsAffected > 0;
            }
        }
        public static async Task<bool> IsPersonExistsByIdAsync(Guid personId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.IsPersonExistsById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PersonId", personId);

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToBoolean(result);
            }
        }
        public static async Task<bool> IsPersonExistsByNameAsync(string firstName, string secondName, string? thirdName=null)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            using (var command = new SqlCommand("dbo.IsPersonExistsByName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@SecondName", secondName);
                command.Parameters.AddWithValue("@ThirdName", (object?)thirdName ?? DBNull.Value);

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToBoolean(result);
            }
        }
        public static async Task<PersonDTO?> GetPersonByNameAsync(string firstName, string secondName, string? thirdName = null)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.GetPersonByName", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", (object?)thirdName ?? DBNull.Value);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PersonDTO(
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
            }

            return null;
        }
        public static async Task<PersonProfileDto?> GetProfileByIdAsync(Guid personId)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetProfileByEntityId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EntityId", personId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new PersonProfileDto
                {
                    FullName = reader["FullName"].ToString() ?? string.Empty,
                    ImageLocation = reader["ImageLocation"].ToString() ?? string.Empty,
                    CurrentRole = reader["CurrentRole"].ToString()??string.Empty
                };
            }

            return null;
        }


    }

}