using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{

    public class ServicePaymentDto
    {
        public int PaymentId { get; set; }
        public Guid PatientId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }

    public static class ServicePaymentData
    {
        public static async Task<bool> InsertServicePaymentAsync(ServicePaymentDto payment, string? stripeSessionId, string? stripePaymentIntentId)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("InsertServicePayment", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PatientId", payment.PatientId);
            cmd.Parameters.AddWithValue("@ServiceType", payment.ServiceType);
            cmd.Parameters.AddWithValue("@ServiceId", payment.ServiceId);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
            cmd.Parameters.AddWithValue("@StripeSessionId", (object?)stripeSessionId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StripePaymentIntentId", (object?)stripePaymentIntentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object?)payment.Notes ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<List<ServicePaymentDto>> GetPaymentsByPatientIdAsync(Guid patientId)
        {
            var result = new List<ServicePaymentDto>();
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("GetPaymentsByPatientId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new ServicePaymentDto
                {
                    PatientId = patientId,
                    PaymentId = reader.GetInt32(0),
                    ServiceType = reader.GetString(1),
                    ServiceId = reader.GetInt32(2),
                    Amount = reader.GetDecimal(3),
                    PaymentMethod = reader.GetString(4),
                    PaymentStatus = reader.GetString(5),
                    PaymentDate = reader.GetDateTime(6),
                    Notes = reader.IsDBNull(7) ? null : reader.GetString(7)
                });
            }

            return result;
        }

        public static async Task<bool> UpdateServicePaymentAsync(int paymentId, string paymentStatus, string? notes)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("UpdateServicePayment", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PaymentId", paymentId);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
            cmd.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> DeleteServicePaymentAsync(int paymentId)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("DeleteServicePayment", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PaymentId", paymentId);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public static async Task<bool> UpdatePaymentStatusAsyncBySessionId(string stripeSessionId, string newStatus)
        {
            using var conn = new SqlConnection(AppConfig.ConnectionString);
            using var cmd = new SqlCommand("UPDATE ServicePayments SET PaymentStatus = @Status WHERE StripeSessionId = @SessionId", conn);
            cmd.Parameters.AddWithValue("@SessionId", stripeSessionId);
            cmd.Parameters.AddWithValue("@Status", newStatus);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }



    }






}
