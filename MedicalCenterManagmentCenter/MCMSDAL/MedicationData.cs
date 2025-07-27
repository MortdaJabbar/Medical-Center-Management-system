using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{
    public class MedicationDto
    {
        public int MedicationID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Strength { get; set; } = string.Empty;
        public string DosageForm { get; set; } = string.Empty;
    }

    public static class MedicationData
    {
        public static async Task<bool> InsertMedicationAsync(MedicationDto medication)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("InsertMedication", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Name", medication.Name);
            command.Parameters.AddWithValue("@Description", (object?)medication.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Strength", medication.Strength);
            command.Parameters.AddWithValue("@DosageForm", medication.DosageForm);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> UpdateMedicationAsync(MedicationDto medication)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("UpdateMedication", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@MedicationID", medication.MedicationID);
            command.Parameters.AddWithValue("@Name", medication.Name);
            command.Parameters.AddWithValue("@Description", (object?)medication.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Strength", medication.Strength);
            command.Parameters.AddWithValue("@DosageForm", medication.DosageForm);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<MedicationDto?> GetMedicationByIdAsync(int id)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetMedicationByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@MedicationID", id);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new MedicationDto
                {
                    MedicationID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Strength = reader.GetString(3),
                    DosageForm = reader.GetString(4)
                };
            }
            return null;
        }

        public static async Task<List<MedicationDto>> GetAllMedicationsAsync( )
        {
            var result = new List<MedicationDto>();
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetAllMedications", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

           

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new MedicationDto
                {
                    MedicationID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Strength = reader.GetString(3),
                    DosageForm = reader.GetString(4)
                });
            }

            return result;
        }

        public static async Task<bool> DeleteMedicationAsync(int id)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("DeleteMedication", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@MedicationID", id);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int affected = reader.GetInt32(0);
                return affected > 0;
            }

            return false;
        }
    }

}
