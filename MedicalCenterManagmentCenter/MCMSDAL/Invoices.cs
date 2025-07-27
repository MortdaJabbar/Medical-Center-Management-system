using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{
    public class UnpaidServiceDto
    {
        public int ServiceID { get; set; }
        public Guid PatientID { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? PatientImage { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InvoiceDetailsDto
    {
        public int InvoiceID { get; set; }
        public Guid PatientID { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? PatientImage { get; set; }
        public string ServiceType { get; set; } = string.Empty; // Test / Appointment / Prescription
        public int ServiceID { get; set; }
        public string ServiceDescription { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
        public string? CardSessionID { get; set; }
        public string? CardPaymentIntentID { get; set; }
    }
    public class AddInvoiceDto
    {
        public Guid PatientID { get; set; }
        public string ServiceType { get; set; } = string.Empty; // Test, Appointment, Prescription
        public int ServiceID { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string PaymentStatus { get; set; } = "Pending";
        public string? Notes { get; set; }
        public string? CardSessionID { get; set; }
        public string? CardPaymentIntentID { get; set; }
    }
    public class UpdateInvoiceDto
    {
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string? Notes { get; set; }
    }
    public class PatientInvoiceDto
    {
        public int InvoiceID { get; set; }
        public Guid PatientID { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public int ServiceID { get; set; }
        public string ServiceDescription { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
    }

    public static class InvoiceData
    {
        public static async Task<List<InvoiceDetailsDto>> GetAllAsync()
        {
            var list = new List<InvoiceDetailsDto>();
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetAllInvoicesDetailed", conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new InvoiceDetailsDto
                {
                    InvoiceID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    PatientName = reader.GetString(2),
                    PatientImage = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ServiceType = reader.GetString(4),
                    ServiceID = reader.GetInt32(5),
                    TotalAmount = reader.GetDecimal(6),
                    PaymentMethod = reader.GetString(7),
                    PaymentStatus = reader.GetString(8),
                    CreatedAt = reader.GetDateTime(9),
                    Notes = reader.IsDBNull(10) ? null : reader.GetString(10),
                    ServiceDescription = reader.GetString(11)
                });
            }

            return list;
        }

        public static async Task<bool> InsertAsync(AddInvoiceDto dto)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("InsertInvoice", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@PatientID", dto.PatientID);
            cmd.Parameters.AddWithValue("@ServiceType", dto.ServiceType);
            cmd.Parameters.AddWithValue("@ServiceID", dto.ServiceID);
            cmd.Parameters.AddWithValue("@TotalAmount", dto.TotalAmount);
            cmd.Parameters.AddWithValue("@PaymentMethod", dto.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentStatus", dto.PaymentStatus);
            cmd.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CardSessionID", (object?)dto.CardSessionID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CardPaymentIntentID", (object?)dto.CardPaymentIntentID ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> UpdateAsync(int InvoiceID, UpdateInvoiceDto dto)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("UpdateInvoice", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@InvoiceID", InvoiceID);
            cmd.Parameters.AddWithValue("@TotalAmount", dto.TotalAmount);
            cmd.Parameters.AddWithValue("@PaymentStatus", dto.PaymentStatus);
            cmd.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);

            await conn.OpenAsync();
            int rowsAffected = (int) await cmd.ExecuteScalarAsync();
            return rowsAffected  > 0;
        }

        public static async Task<bool> DeleteAsync(int invoiceId)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("DeleteInvoice", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);

            await conn.OpenAsync();
            int rowsAffected = (int)await cmd.ExecuteScalarAsync();
            return rowsAffected > 0;
        }

        public static async Task<List<PatientInvoiceDto>> GetInvoicesForPatientAsync(Guid patientId)
        {
            var list = new List<PatientInvoiceDto>();

            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetInvoicesByPatientId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@PatientID", patientId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PatientInvoiceDto
                {
                    InvoiceID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    ServiceType = reader.GetString(2),
                    ServiceID = reader.GetInt32(3),
                    TotalAmount = reader.GetDecimal(4),
                    PaymentMethod = reader.GetString(5),
                    PaymentStatus = reader.GetString(6),
                    CreatedAt = reader.GetDateTime(7),
                    Notes = reader.IsDBNull(8) ? null : reader.GetString(8),
                    ServiceDescription = reader.GetString(9)
                });
            }

            return list;
        }


        public static async Task<List<UnpaidServiceDto>> GetUnpaidTestsAsync() => await GetUnpaid("GetUnpaidTests");
        public static async Task<List<UnpaidServiceDto>> GetUnpaidAppointmentsAsync() => await GetUnpaid("GetUnpaidAppointments");
        public static async Task<List<UnpaidServiceDto>> GetUnpaidPrescriptionsAsync() => await GetUnpaid("GetUnpaidPrescriptions");

        private static async Task<List<UnpaidServiceDto>> GetUnpaid(string spName)
        {
            var list = new List<UnpaidServiceDto>();
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new UnpaidServiceDto
                {
                    ServiceID = reader.GetInt32(0),
                    PatientID = reader.GetGuid(1),
                    PatientName = reader.GetString(2),
                    PatientImage = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ServiceName = reader.GetString(4),
                    TotalAmount = reader.GetDecimal(5),
                    CreatedAt = reader.GetDateTime(6)
                });
            }

            return list;
        }


    }





}
