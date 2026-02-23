using MCMSDAL;
using MCMSDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class ServicePayment : IServicePayment
    {
        private readonly IServicePaymentData _servicePaymentData;

        public ServicePayment(IServicePaymentData servicePaymentData)
        {
            _servicePaymentData = servicePaymentData;
        }

        public async Task<bool> AddPaymentAsync(ServicePaymentDto payment, string? stripeSessionId, string? stripePaymentIntentId)
        {
            return await _servicePaymentData.InsertServicePaymentAsync(payment, stripeSessionId, stripePaymentIntentId);
        }

        public async Task<List<ServicePaymentDto>> GetPaymentsForPatientAsync(Guid patientId)
        {
            return await _servicePaymentData.GetPaymentsByPatientIdAsync(patientId);
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string newStatus, string? notes)
        {
            return await _servicePaymentData.UpdateServicePaymentAsync(paymentId, newStatus, notes);
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            return await _servicePaymentData.DeleteServicePaymentAsync(paymentId);
        }

        public async Task<bool> MarkPaymentCompletedFromStripeAsync(string stripeSessionId)
        {
            return await _servicePaymentData.UpdatePaymentStatusAsyncBySessionId(stripeSessionId, "Completed");
        }


    }
}
