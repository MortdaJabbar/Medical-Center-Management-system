using Microsoft.Data.SqlClient;
using System.Data;


namespace MCMSDAL
{
    
    public  class UpdateTestDto
    {
        public int Stauts { get; set; }
      public string? Notes { get; set; }
      public string? TestResult { get; set; }
      public decimal Cost { get; set; }
    
    }
    public class TestDetailsDto
    {
        public int TestID { get; set; }

        public Guid PatientID { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public string? PatientImage { get; set; }

        public Guid DoctorID { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string? DoctorImage { get; set; }

        public int TestTypeID { get; set; }

        public DateOnly CreatedAt { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal Cost { get; set; }

        public string? TestResult { get; set; }
    }
    public class TestDoctorDto 
    {

        public int TestID { get; set; }
        public int TestTypeID { get; set; }
        public DateOnly CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal Cost { get; set; }
        public string? TestResult { get; set; }

        public string TestTypeName { get; set; } = string.Empty;

        public Guid PatientID { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientImage { get; set; } = string.Empty;
    }
    public class TestPatientsDto
    {
        public int TestID { get; set; }
        public int TestTypeID { get; set; }
        public DateOnly CreatedAt { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
        public decimal Cost { get; set; }
        public string? TestResult { get; set; }

        public string TestTypeName { get; set; } = string.Empty;

        public Guid DoctorID { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorImage { get; set; } = string.Empty;
    }

    public class TestDto
    {
        public int TestID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public int TestTypeID { get; set; }
        public DateOnly CreatedAt { get; set; }
        public int Status { get; set; } 
        public string? Notes { get; set; }
        public decimal Cost { get; set; }
        public string? TestResult { get; set; }
    }
    public class TestData
    {
        public static async Task<int> CreateTestAsync(TestDto test)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.InsertTest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PatientID", test.PatientID);
                    command.Parameters.AddWithValue("@DoctorID", test.DoctorID);
                    command.Parameters.AddWithValue("@TestTypeID", test.TestTypeID);
                    command.Parameters.AddWithValue("@CreatedAt", test.CreatedAt);
                    command.Parameters.AddWithValue("@Status", test.Status);
                    command.Parameters.AddWithValue("@Notes", (object)test.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cost", test.Cost);
                    command.Parameters.AddWithValue("@TestResult", (object)test.TestResult ?? DBNull.Value);

                    var outputTestId = new SqlParameter("@TestID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputTestId);

                    await command.ExecuteNonQueryAsync();

                    return (int)outputTestId.Value;
                }
            }
        }
        public static async Task<TestDto?> GetTestByIdAsync(int testId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.GetTestByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TestID", testId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new TestDto
                            {
                                TestID = testId,
                                PatientID = reader.GetGuid(reader.GetOrdinal("PatientID")),
                                DoctorID = reader.GetGuid(reader.GetOrdinal("DoctorID")),
                                TestTypeID = reader.GetInt32(reader.GetOrdinal("TestTypeID")),
                                CreatedAt = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("CreatedAt"))),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                Cost = reader.GetDecimal(reader.GetOrdinal("Cost")),
                                TestResult = reader.IsDBNull(reader.GetOrdinal("TestResult")) ? null : reader.GetString(reader.GetOrdinal("TestResult")),
                                Status = reader.GetInt32(reader.GetOrdinal("Status"))

                            };
                        }
                    }
                }
            }
            return null;
        }
        public static async Task<bool> UpdateTestAsync(TestDto test)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.UpdateTest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TestID", test.TestID);
                    command.Parameters.AddWithValue("@PatientID", test.PatientID);
                    command.Parameters.AddWithValue("@DoctorID", test.DoctorID);
                    command.Parameters.AddWithValue("@TestTypeID", test.TestTypeID);
                    command.Parameters.AddWithValue("@Status", test.Status);
                    command.Parameters.AddWithValue("@CreatedAt", test.CreatedAt);
                    command.Parameters.AddWithValue("@Notes", (object)test.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cost", test.Cost);
                    command.Parameters.AddWithValue("@TestResult", (object)test.TestResult ?? DBNull.Value);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
        public static async Task<bool> DeleteTestAsync(int testId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.DeleteTest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TestID", testId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
        public static async Task<bool> IsTestExistsByIdAsync(int testId)
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.IsTestExistsById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TestID", testId);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToBoolean(result);
                }
            }
        }
        public static async Task<List<TestDetailsDto>> GetAllTestsAsync()
        {
            var tests = new List<TestDetailsDto>();

            using (var connection = new SqlConnection(AppConfig.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.GetAllTests", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                   

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tests.Add(new TestDetailsDto
                            {
                                TestID = reader.GetInt32(reader.GetOrdinal("TestID")),
                                PatientID = reader.GetGuid(reader.GetOrdinal("PatientID")),
                                DoctorID = reader.GetGuid(reader.GetOrdinal("DoctorID")),
                                TestTypeID = reader.GetInt32(reader.GetOrdinal("TestTypeID")),
                                CreatedAt = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("CreatedAt"))),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                Cost = reader.GetDecimal(reader.GetOrdinal("Cost")),
                                TestResult = reader.IsDBNull(reader.GetOrdinal("TestResult")) ? null : reader.GetString(reader.GetOrdinal("TestResult")),
                                DoctorImage = reader.GetString(reader.GetOrdinal("DoctorImage")),
                                PatientImage = reader.GetString(reader.GetOrdinal("PatientImage")),
                                DoctorFullName = reader.GetString(reader.GetOrdinal("DoctorFullName")),
                                PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
                                TestName = reader.GetString(reader.GetOrdinal("TestName"))
                            });
                        }
                    }
                }
            }
            return tests;
        }
        public static async Task<List<TestDoctorDto>> GetTestsByDoctorIdAsync(Guid doctorId)
        {
            var result = new List<TestDoctorDto>();

            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetTestsByDoctorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new TestDoctorDto
                {
                    TestID = reader.GetInt32(0),
                    TestTypeID = reader.GetInt32(1),
                    CreatedAt = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    Status = reader.GetString(3),
                    Notes = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Cost = reader.GetDecimal(5),
                    TestResult = reader.IsDBNull(6) ? null : reader.GetString(6),
                    TestTypeName = reader.GetString(7),
                    PatientID = reader.GetGuid(8),
                    PatientName = reader.GetString(9),
                    PatientImage = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                });
            }

            return result;
        }
        public static async Task<List<TestPatientsDto>> GetTestsByPatientIdAsync(Guid patientId)
        {
            var result = new List<TestPatientsDto>();

            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetTestsByPatientId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PatientId", patientId);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new TestPatientsDto
                {
                    TestID = reader.GetInt32(0),
                    TestTypeID = reader.GetInt32(1),
                    CreatedAt =DateOnly.FromDateTime( reader.GetDateTime(2)),
                    Status = reader.GetString(3),
                    Notes = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Cost = reader.GetDecimal(5),
                    TestResult = reader.IsDBNull(6) ? null : reader.GetString(6),
                    TestTypeName = reader.GetString(7),
                    DoctorID = reader.GetGuid(8),
                    DoctorName = reader.GetString(9),
                    DoctorImage = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                });
            }

            return result;
        }
        public static async Task<List<PatientDoctorDto>> GetPatientDoctorPairsAsync()
        {
            var list = new List<PatientDoctorDto>();

            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetPatientDoctorPairs", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new PatientDoctorDto
                {
                    PatientId = reader.GetGuid(reader.GetOrdinal("PatientId")),
                    PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
                    DoctorId = reader.GetGuid(reader.GetOrdinal("DoctorId")),
                    DoctorFullName = reader.GetString(reader.GetOrdinal("DoctorFullName"))
                });
            }

            return list;
        }
         
    
    
    }
}

