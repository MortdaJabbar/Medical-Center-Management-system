using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IServicePaymentData
    {
        Task<bool> InsertServicePaymentAsync(ServicePaymentDto payment, string? stripeSessionId, string? stripePaymentIntentId);
        Task<List<ServicePaymentDto>> GetPaymentsByPatientIdAsync(Guid patientId);
        Task<bool> UpdateServicePaymentAsync(int paymentId, string paymentStatus, string? notes);
        Task<bool> DeleteServicePaymentAsync(int paymentId);
        Task<bool> UpdatePaymentStatusAsyncBySessionId(string stripeSessionId, string newStatus);
    }
}
