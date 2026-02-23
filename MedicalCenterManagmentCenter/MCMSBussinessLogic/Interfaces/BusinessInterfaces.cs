using MCMSDAL;
using Stripe.Checkout;

namespace MCMSBussinessLogic
{
    public interface IPerson
    {
        Task<bool> AddNewPersonAsync();
        Task<bool> UpdatePersonAsync();
    }

    public interface IDoctor
    {
        Task<bool> AddNewDoctorAsync();
        Task<bool> UpdateDoctorAsync();
    }

    public interface IPatient
    {
        Task<bool> AddNewPatientAsync();
        Task<bool> UpdatePatientAsync();
    }

    public interface IPharmacist
    {
        Task<bool> AddNewPharmacistAsync();
        Task<bool> UpdatePharmacistAsync();
    }

    public interface IStaff
    {
        Task<bool> AddNewStaffAsync();
        Task<bool> UpdateStaffAsync();
    }

    public interface IAppointment
    {
        Task<bool> AddNewAppointmentAsync();
        Task<bool> UpdateAppointmentAsync();
    }

    public interface IInventory
    {
        Task<bool> AddNewInventoryAsync();
        Task<bool> UpdateInventoryAsync();
    }

    public interface IMedication
    {
        Task<bool> AddNewMedicationAsync();
        Task<bool> UpdateMedicationAsync();
    }

    public interface IPrescription
    {
        Task<bool> AddNewPrescriptionAsync();
        Task<bool> UpdatePrescriptionAsync();
    }

    public interface ITest
    {
        Task<bool> AddNewTestAsync();
        Task<bool> UpdateTestAsync();
    }

    public interface ITestType
    {
        Task<bool> AddNewTestTypeAsync();
        Task<bool> UpdateTestTypeAsync();
    }

    public interface IUserAccount
    {
        Task<bool> RegisterAsync();
        Task<bool> ActivateUserAsync();
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<bool> UpdateAsync();
        Task<bool> ResetPassword();
    }

    public interface IServicePayment
    {
        Task<bool> AddPaymentAsync(ServicePaymentDto payment, string? stripeSessionId, string? stripePaymentIntentId);
        Task<List<ServicePaymentDto>> GetPaymentsForPatientAsync(Guid patientId);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, string newStatus, string? notes);
        Task<bool> DeletePaymentAsync(int paymentId);
        Task<bool> MarkPaymentCompletedFromStripeAsync(string stripeSessionId);
    }

    public interface IStripeService
    {
        Session CreateStripeSession(decimal amount, string successUrl, string cancelUrl);
    }

    public interface IInvoice
    {
        Task<List<InvoiceDetailsDto>> GetAllInvoicesAsync();
        Task<bool> InsertInvoiceAsync(AddInvoiceDto dto);
        Task<bool> UpdateInvoiceAsync(int invoiceId, UpdateInvoiceDto dto);
        Task<bool> DeleteInvoiceAsync(int invoiceId);
        Task<List<PatientInvoiceDto>> GetInvoicesByPatientIdAsync(Guid patientId);
        Task<List<UnpaidServiceDto>> GetUnpaidTestsAsync();
        Task<List<UnpaidServiceDto>> GetUnpaidAppointmentsAsync();
        Task<List<UnpaidServiceDto>> GetUnpaidPrescriptionsAsync();
    }
}