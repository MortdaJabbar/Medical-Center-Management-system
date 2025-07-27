using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{
    public class PrescriptionDetailsDto
    {
        public int PrescriptionID { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public int? Refills { get; set; }
        public string? Instructions { get; set; }
        public string MedicationName { get; set; }
        public int MedicationID { get; set; }
       
        public Guid PatientID { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public string? PatientImage { get; set; }

        public Guid DoctorID { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string? DoctorImage { get; set; }
    }
    public class PrescriptionByDoctorDto
    {
        public int PrescriptionId { get; set; }
        public DateOnly PrescriptionDate { get; set; }
        public int Refills { get; set; }
        public string Instructions { get; set; }

        public string MedicationName { get; set; }

        public string PatientFullName { get; set; }
        public string PatientImage { get; set; }
    }

    public class UpdatePrescriptionDto 
    {
      public int  MedicationID { get; set; }
      public DateOnly PrescriptionDate { get; set; }
      public int refills { get; set; }
      public string instructions { get; set; }


    }
    public class PrescriptionPatientDto
    {
        public int PrescriptionID { get; set; }
        public DateOnly PrescriptionDate { get; set; }
        public int Refills { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string MedicationName { get; set; } = string.Empty;
        public Guid DoctorID { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorImage { get; set; } = string.Empty;
        public Guid? PharmacistID { get; set; }
        public string? PharmacistName { get; set; }
        public string? PharmacistImage { get; set; }
    }

    public class PrescriptionDto
    {
        public int PrescriptionID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public int MedicationID { get; set; }
        public DateOnly PrescriptionDate { get; set; }
        public int? Refills { get; set; }
        public string? Instructions { get; set; }
    }
    public static class PrescriptionData
    {
        public static async Task<int> CreatePrescriptionAsync(PrescriptionDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("InsertPrescription", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PatientID", dto.PatientID);
            command.Parameters.AddWithValue("@DoctorID", dto.DoctorID);
            command.Parameters.AddWithValue("@MedicationID", dto.MedicationID);
            command.Parameters.AddWithValue("@PrescriptionDate", dto.PrescriptionDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@Refills", (object?)dto.Refills ?? DBNull.Value);
            command.Parameters.AddWithValue("@Instructions", (object?)dto.Instructions ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public static async Task<bool> UpdatePrescriptionAsync(PrescriptionDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("UpdatePrescription", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PrescriptionID", dto.PrescriptionID);
            command.Parameters.AddWithValue("@PatientID", dto.PatientID);
            command.Parameters.AddWithValue("@DoctorID", dto.DoctorID);
            command.Parameters.AddWithValue("@MedicationID", dto.MedicationID);
            command.Parameters.AddWithValue("@PrescriptionDate", dto.PrescriptionDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@Refills", (object?)dto.Refills ?? DBNull.Value);
            command.Parameters.AddWithValue("@Instructions", (object?)dto.Instructions ?? DBNull.Value);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetPrescriptionByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PrescriptionID", id);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PrescriptionDto
                {
                    PrescriptionID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    DoctorID = reader.GetGuid(2),
                    MedicationID = reader.GetInt32(3),
                    PrescriptionDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    Refills = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    Instructions = reader.IsDBNull(6) ? null : reader.GetString(6)
                };
            }

            return null;
        }

        public static async Task<List<PrescriptionDto>> GetAllPrescriptionsAsync()
        {
            var list = new List<PrescriptionDto>();
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetAllPrescriptions", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new PrescriptionDto
                {
                    PrescriptionID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    DoctorID = reader.GetGuid(2),
                    MedicationID = reader.GetInt32(3),
                    PrescriptionDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    Refills = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    Instructions = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }

            return list;
        }

        public static async Task<List<PrescriptionDto>> GetPagedPrescriptionsAsync(int pageNumber, int pageSize)
        {
            var list = new List<PrescriptionDto>();
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetPagedPrescriptions", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new PrescriptionDto
                {
                    PrescriptionID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    DoctorID = reader.GetGuid(2),
                    MedicationID = reader.GetInt32(3),
                    PrescriptionDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    Refills = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    Instructions = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }

            return list;
        }

        public static async Task<bool> DeletePrescriptionAsync(int id)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("DeletePrescription", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PrescriptionID", id);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int affected = reader.GetInt32(0);
                return affected > 0;
            }

            return false;
        }

        public static async Task<  List<PrescriptionPatientDto>  > GetPrescriptionsByPatientIdAsync(Guid patientId)
        {
            var prescriptions = new List<PrescriptionPatientDto>();


            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetPrescriptionsByPatientId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@PatientID", patientId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                prescriptions.Add(new PrescriptionPatientDto
                {
                    PrescriptionID = reader.GetInt32(reader.GetOrdinal("PrescriptionID")),
                    PrescriptionDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("PrescriptionDate"))),
                    Refills = reader.GetInt32(reader.GetOrdinal("Refills")),
                    Instructions = reader.GetString(reader.GetOrdinal("Instructions")),
                    MedicationName = reader.GetString(reader.GetOrdinal("MedicationName")),
                    DoctorID = reader.GetGuid(reader.GetOrdinal("DoctorID")),
                    DoctorName = reader.GetString(reader.GetOrdinal("DoctorName")),
                    DoctorImage = reader.GetString(reader.GetOrdinal("DoctorImage")),
                    PharmacistID = reader.IsDBNull(reader.GetOrdinal("PharmacistID")) ? null : reader.GetGuid(reader.GetOrdinal("PharmacistID")),
                    PharmacistName = reader.IsDBNull(reader.GetOrdinal("PharmacistName")) ? null : reader.GetString(reader.GetOrdinal("PharmacistName")),
                    PharmacistImage = reader.GetString(reader.GetOrdinal("PharmacistImage"))

                });
            }

            return prescriptions;
        }
        public static async Task<List<PrescriptionDetailsDto>> GetAllWithNamesAsync()
        {
            var prescriptions = new List<PrescriptionDetailsDto>();

            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetAllPrescriptionsWithNames", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                prescriptions.Add(new PrescriptionDetailsDto
                {
                    PrescriptionID = reader.GetInt32(0),
                    PrescriptionDate = reader.GetDateTime(1),
                    Refills = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    Instructions = reader.IsDBNull(3) ? null : reader.GetString(3),
                    MedicationID = reader.GetInt32(4),

                    PatientID = reader.GetGuid(5),
                    PatientFullName = reader.GetString(6),
                    PatientImage = reader.IsDBNull(7) ? null : reader.GetString(7),

                    DoctorID = reader.GetGuid(8),
                    DoctorFullName = reader.GetString(9),
                    DoctorImage = reader.IsDBNull(10) ? null : reader.GetString(10),
                    MedicationName = reader.GetString(11)
                });
            }

            return prescriptions;
        }

        public static  async Task<List<PrescriptionByDoctorDto>> GetPrescriptionsByDoctorIdAsync(Guid doctorId)
        {
            var list = new List<PrescriptionByDoctorDto>();

            using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("GetPrescriptionsByDoctorId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                await con.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new PrescriptionByDoctorDto
                        {
                            PrescriptionId = (int)reader["PrescriptionId"],
                            PrescriptionDate = DateOnly.FromDateTime((DateTime)reader["PrescriptionDate"]),
                            Refills = (int)reader["Refills"],
                            Instructions = reader["Instructions"].ToString(),

                            MedicationName = reader["MedicationName"].ToString(),
                            PatientFullName = reader["PatientFullName"].ToString(),
                            PatientImage = reader["PatientImage"].ToString()
                        });
                    }
                }
            }

            return list;
        }




    }





}
