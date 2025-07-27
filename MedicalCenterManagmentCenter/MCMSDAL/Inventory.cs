using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{
    public class InventoryDisplayDto
    {
        public int InventoryID { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public DateOnly ExpiryDate { get; set; }
        public DateOnly RecivedDate { get; set; }
    }


    public class InventoryDto
    {
        public int InventoryID { get; set; }

        public int MedicationID { get; set; }

        public int Quantity { get; set; }

        public string? UnitOfMeasure { get; set; }

        public DateOnly ExpiryDate { get; set; }

        public string? Supplier { get; set; }

        public DateOnly RecivedDate { get; set; }
    }



    public static class InventoryData
    {
        public static async Task<int> CreateInventoryAsync(InventoryDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("InsertInventory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@MedicationID", dto.MedicationID);
            command.Parameters.AddWithValue("@Quantity", dto.Quantity);
            command.Parameters.AddWithValue("@UnitOfMeasure", (object?)dto.UnitOfMeasure ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", dto.ExpiryDate);
            command.Parameters.AddWithValue("@Supplier", (object?)dto.Supplier ?? DBNull.Value);
            command.Parameters.AddWithValue("@RecivedDate", dto.RecivedDate);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public static async Task<bool> UpdateInventoryAsync(InventoryDto dto)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("UpdateInventory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@InventoryID", dto.InventoryID);
            command.Parameters.AddWithValue("@MedicationID", dto.MedicationID);
            command.Parameters.AddWithValue("@Quantity", dto.Quantity);
            command.Parameters.AddWithValue("@UnitOfMeasure", (object?)dto.UnitOfMeasure ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", dto.ExpiryDate);
            command.Parameters.AddWithValue("@Supplier", (object?)dto.Supplier ?? DBNull.Value);
            command.Parameters.AddWithValue("@RecivedDate", dto.RecivedDate);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<InventoryDto?> GetInventoryByIdAsync(int inventoryId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetInventoryByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@InventoryID", inventoryId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new InventoryDto
                {
                    InventoryID = reader.GetInt32(0),
                    MedicationID = reader.GetInt32(1),
                    Quantity = reader.GetInt32(2),
                    UnitOfMeasure = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ExpiryDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    Supplier = reader.IsDBNull(5) ? null : reader.GetString(5),
                    RecivedDate = DateOnly.FromDateTime(reader.GetDateTime(6))
                };
            }

            return null;
        }

        public static async Task<List<InventoryDto>> GetAllInventoryAsync()
        {
            var list = new List<InventoryDto>();
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("GetAllInventory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new InventoryDto
                {
                    InventoryID = reader.GetInt32(0),
                    MedicationID = reader.GetInt32(1),
                    Quantity = reader.GetInt32(2),
                    UnitOfMeasure = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ExpiryDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    Supplier = reader.IsDBNull(5) ? null : reader.GetString(5),
                    RecivedDate = DateOnly.FromDateTime(reader.GetDateTime(6)),
                    
                });
            }

            return list;
        }

        public static async Task<bool> DeleteInventoryAsync(int inventoryId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("DeleteInventory", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@InventoryID", inventoryId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int rows = reader.GetInt32(0);
                return rows > 0;
            }

            return false;
        }

        public static async Task<List<InventoryDisplayDto>> GetAllInventoryDetailsAsync()
        {
            var list = new List<InventoryDisplayDto>();

            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetAllInventoryWithMedicationNames", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new InventoryDisplayDto
                {
                    InventoryID = Convert.ToInt32(reader["InventoryID"]),
                    MedicationName = reader["MedicationName"].ToString()!,
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    UnitOfMeasure = reader["UnitOfMeasure"].ToString()!,
                    Supplier = reader["Supplier"].ToString()!,

                    ExpiryDate = DateOnly.FromDateTime(reader.GetDateTime(5)),
                    RecivedDate = DateOnly.FromDateTime(reader.GetDateTime(6))
                });
            }

            return list;
        }



    }

}
