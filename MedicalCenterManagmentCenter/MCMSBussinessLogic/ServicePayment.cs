using MCMSDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class ServicePayment
    {

        public async Task<bool> AddPaymentAsync(ServicePaymentDto payment, string? stripeSessionId, string? stripePaymentIntentId)
        {
            return await ServicePaymentData.InsertServicePaymentAsync(payment, stripeSessionId, stripePaymentIntentId);
        }

        public async Task<List<ServicePaymentDto>> GetPaymentsForPatientAsync(Guid patientId)
        {
            return await ServicePaymentData.GetPaymentsByPatientIdAsync(patientId);
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string newStatus, string? notes)
        {
            return await ServicePaymentData.UpdateServicePaymentAsync(paymentId, newStatus, notes);
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            return await ServicePaymentData.DeleteServicePaymentAsync(paymentId);
        }

        public async Task<bool> MarkPaymentCompletedFromStripeAsync(string stripeSessionId)
        {
            return await ServicePaymentData.UpdatePaymentStatusAsyncBySessionId(stripeSessionId, "Completed");
        }


    }
}
