using System.Data;
using Microsoft.Data.SqlClient;

namespace MCMSDAL
{

    public class TestTypeSummaryDto
    {
        public int TestTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TestTypeDto
    {
        public int TestTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
    }
    public class TestTypeData
    {
        

        // Method to create a TestType (InsertTestType stored procedure)
        public async static Task<int> CreateTestType(TestTypeDto testType)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.InsertTestType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters using the object properties
                    command.Parameters.AddWithValue("@Name", testType.Name);
                    command.Parameters.AddWithValue("@Description", (object)testType.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cost", testType.Cost);

                    // Output parameter for TestTypeId
                    var outputTestTypeId = new SqlParameter("@TestTypeId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputTestTypeId);

                    command.ExecuteNonQuery();

                    // Return the new TestTypeId
                    return (int)outputTestTypeId.Value;
                }
            }
        }

        // Method to get a TestType by ID (GetTestTypeById stored procedure)
        public async static Task<TestTypeDto> GetTestTypeById(int testTypeId)
        {
            TestTypeDto? testType = null;

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.GetTestTypeById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TestTypeId", testTypeId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            testType = new TestTypeDto
                            {
                                TestTypeId = reader.GetInt32(reader.GetOrdinal("TestTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Cost = reader.GetDecimal(reader.GetOrdinal("Cost"))
                            };
                        }
                    }
                }
            }

            return testType;
        }

        // Method to update a TestType (UpdateTestType stored procedure)
        public async static Task<bool> UpdateTestType(TestTypeDto testType)
        {
            if (! await IsTestTypeExistsById(testType.TestTypeId)) { return false; }

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.UpdateTestType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters using the object properties
                    command.Parameters.AddWithValue("@TestTypeId", testType.TestTypeId);
                    command.Parameters.AddWithValue("@Name", testType.Name);
                    command.Parameters.AddWithValue("@Description", (object)testType.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cost", testType.Cost);

                    int rowsAffected = (int)command.ExecuteScalar();

                    return rowsAffected > 0;
                }
            }
        }

     
        public async static Task<bool> DeleteTestType(int testTypeId)
        {
            if (!await IsTestTypeExistsById(testTypeId)) { return false; }

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.DeleteTestType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TestTypeId", testTypeId);

                    int rowsAffected = (int)command.ExecuteScalar();
                    return rowsAffected > 0;
                }
            }
        }

        // Method to check if a TestType exists by ID (IsTestTypeExistsById stored procedure)
        public async static Task<bool> IsTestTypeExistsById(int testTypeId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.IsTestTypeExistsById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TestTypeId", testTypeId);

                    var result = command.ExecuteScalar();
                    return Convert.ToBoolean(result);
                }
            }
        }
        public async static Task<bool> IsTestTypeExistsByName(string  Name)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.IsTestTypeExistsByName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", Name);

                    var result = command.ExecuteScalar();
                    return Convert.ToBoolean(result);
                }
            }
        }
        // Method to retrieve all TestTypes (GetAllTestTypes stored procedure)
        public async static  Task<List<TestTypeDto>> GetAllTestTypes()
        {
            var testTypes = new List<TestTypeDto>();

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.GetAllTestTypes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var testType = new TestTypeDto
                            {
                                TestTypeId = reader.GetInt32(reader.GetOrdinal("TestTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Cost = reader.GetDecimal(reader.GetOrdinal("Cost"))
                            };

                            testTypes.Add(testType);
                        }
                    }
                }
            }

            return testTypes;
        }
    }

}
