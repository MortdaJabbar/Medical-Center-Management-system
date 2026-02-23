using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IInvoiceData
    {
        Task<List<InvoiceDetailsDto>> GetAllAsync();
        Task<bool> InsertAsync(AddInvoiceDto dto);
        Task<bool> UpdateAsync(int invoiceId, UpdateInvoiceDto dto);
        Task<bool> DeleteAsync(int invoiceId);
        Task<List<PatientInvoiceDto>> GetInvoicesForPatientAsync(Guid patientId);
        Task<List<UnpaidServiceDto>> GetUnpaidTestsAsync();
        Task<List<UnpaidServiceDto>> GetUnpaidAppointmentsAsync();
        Task<List<UnpaidServiceDto>> GetUnpaidPrescriptionsAsync();
    }
}
