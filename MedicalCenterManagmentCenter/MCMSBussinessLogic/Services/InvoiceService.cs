using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceData _invoiceData;

        public InvoiceService(IInvoiceData invoiceData)
        {
            _invoiceData = invoiceData;
        }

        public Task<List<InvoiceDetailsDto>> GetAllAsync()
        {
            return _invoiceData.GetAllAsync();
        }

        public Task<List<PatientInvoiceDto>> GetByPatientIdAsync(Guid patientId)
        {
            return _invoiceData.GetInvoicesForPatientAsync(patientId);
        }

        public Task<bool> InsertAsync(AddInvoiceDto dto)
        {
            return _invoiceData.InsertAsync(dto);
        }

        public Task<bool> UpdateAsync(int invoiceId, UpdateInvoiceDto dto)
        {
            return _invoiceData.UpdateAsync(invoiceId, dto);
        }

        public Task<bool> DeleteAsync(int invoiceId)
        {
            return _invoiceData.DeleteAsync(invoiceId);
        }

        public Task<List<UnpaidServiceDto>> GetUnpaidTestsAsync()
        {
            return _invoiceData.GetUnpaidTestsAsync();
        }

        public Task<List<UnpaidServiceDto>> GetUnpaidAppointmentsAsync()
        {
            return _invoiceData.GetUnpaidAppointmentsAsync();
        }

        public Task<List<UnpaidServiceDto>> GetUnpaidPrescriptionsAsync()
        {
            return _invoiceData.GetUnpaidPrescriptionsAsync();
        }
    }
}
