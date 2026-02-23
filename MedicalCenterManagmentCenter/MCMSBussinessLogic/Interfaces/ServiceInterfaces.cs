using MCMSDAL;

namespace MCMSBussinessLogic.Interfaces
{
    public interface IAppointmentService
    {
        Task<bool> AddAsync(Appointment appointment);
        Task<bool> UpdateAsync(Appointment appointment);
        Task<bool> DeleteAsync(int appointmentId);
        Task<Appointment?> FindByIdAsync(int appointmentId);
        Task<List<Appointment>> GetAllAsync();
        Task<List<AppointmentPatientDto>> GetByPatientIdAsync(Guid patientId);
        Task<List<AppointmentSummaryDto>> GetAppointmentsWithDetailsAsync();
    }

    public interface IDoctorService
    {
        Task<Guid?> CreateAsync(Doctor doctor);
        Task<bool> UpdateAsync(Doctor doctor);
        Task<Doctor?> FindByIdAsync(Guid doctorId);
        Task<bool> DeleteAsync(Guid doctorId, Guid personId);
        Task<List<Doctor>> GetAllAsync();
        Task<List<DoctorSummaryDto>> GetSummariesAsync();
        Task<List<AppointmentByDoctorDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<List<TestDoctorDto>> GetTestsByDoctorIdAsync(Guid doctorId);
        Task<List<PrescriptionByDoctorDto>> GetPrescriptionsByDoctorIdAsync(Guid doctorId);
        Task<DoctorDashboardStatsDto?> GetDashboardStatsAsync(Guid doctorId);
    }

    public interface IPatientService
    {
        Task<Guid?> CreateAsync(Patient patient);
        Task<bool> UpdateAsync(Patient patient);
        Task<Patient?> FindByIdAsync(Guid patientId);
        Task<bool> DeleteAsync(Guid patientId, Guid personId);
        Task<List<Patient>> GetAllAsync();
        Task<List<PatientSummaryDto>> GetSummariesAsync();
        Task<List<AppointmentPatientDto>> GetAppointmentsAsync(Guid patientId);
        Task<List<PrescriptionPatientDto>> GetPrescriptionsAsync(Guid patientId);
        Task<List<TestPatientsDto>> GetTestsAsync(Guid patientId);
        Task<PatientDashboardDto?> GetDashboardStatsAsync(Guid patientId);
        Task<List<PatientInvoiceDto>> GetInvoicesAsync(Guid patientId);
    }

    public interface IPharmacistService
    {
        Task<Guid?> CreateAsync(Pharmacist pharmacist);
        Task<bool> UpdateAsync(Pharmacist pharmacist);
        Task<Pharmacist?> FindByIdAsync(Guid pharmacistId);
        Task<bool> DeleteAsync(Guid pharmacistId, Guid personId);
        Task<List<Pharmacist>> GetAllAsync();
        Task<PharmacyDashboardStatsDto> GetPharmacyDashboardStatsAsync();
    }

    public interface IStaffService
    {
        Task<Guid?> CreateAsync(Staff staff);
        Task<bool> UpdateAsync(Staff staff);
        Task<Staff?> FindByIdAsync(Guid staffId);
        Task<bool> DeleteAsync(Guid staffId, Guid personId);
        Task<List<Staff>> GetAllAsync();
        Task<List<StaffSummaryDto>> GetSummariesAsync();
        Task<StaffDashboardStatsDto> GetStaffDashboardStatsAsync();
        Task<AdminDashboardStatsDto> GetAdminDashboardStatsAsync();
    }

    public interface IInventoryService
    {
        Task<int?> CreateAsync(Inventory inventory);
        Task<bool> UpdateAsync(Inventory inventory);
        Task<Inventory?> FindByIdAsync(int inventoryId);
        Task<List<Inventory>> GetAllAsync();
        Task<List<InventoryDisplayDto>> GetAllDetailsAsync();
        Task<bool> DeleteAsync(int inventoryId);
    }

    public interface IMedicationService
    {
        Task<bool> CreateAsync(Medication medication);
        Task<bool> UpdateAsync(Medication medication);
        Task<Medication?> FindByIdAsync(int medicationId);
        Task<List<Medication>> GetAllAsync();
        Task<bool> DeleteAsync(int medicationId);
    }

    public interface IPrescriptionService
    {
        Task<int?> CreateAsync(Prescription prescription);
        Task<bool> UpdateAsync(Prescription prescription);
        Task<Prescription?> FindByIdAsync(int prescriptionId);
        Task<List<Prescription>> GetAllAsync();
        Task<List<Prescription>> GetPagedAsync(int page, int size);
        Task<bool> DeleteAsync(int prescriptionId);
        Task<List<PrescriptionDetailsDto>> GetDetailedAsync();
    }

    public interface ITestService
    {
        Task<int?> CreateAsync(Test test);
        Task<bool> UpdateAsync(Test test);
        Task<Test?> FindByIdAsync(int testId);
        Task<List<TestDetailsDto>> GetPagedAsync(int page, int size);
        Task<List<TestDetailsDto>> GetAllDetailedAsync();
        Task<bool> DeleteAsync(int testId);
        Task<List<TestDoctorDto>> GetByDoctorIdAsync(Guid doctorId);
        Task<List<TestPatientsDto>> GetByPatientIdAsync(Guid patientId);
        Task<List<PatientDoctorDto>> GetPairsAsync();
    }

    public interface ITestTypeService
    {
        Task<int?> CreateAsync(TestType testType);
        Task<bool> UpdateAsync(TestType testType);
        Task<TestType?> FindByIdAsync(int testTypeId);
        Task<List<TestType>> GetAllAsync();
        Task<List<TestType>> GetPagedAsync(int page, int size);
        Task<bool> DeleteAsync(int testTypeId);
        Task<bool> ExistsByNameAsync(string name);
    }

    public interface IInvoiceService
    {
        Task<List<InvoiceDetailsDto>> GetAllAsync();
        Task<List<PatientInvoiceDto>> GetByPatientIdAsync(Guid patientId);
        Task<bool> InsertAsync(AddInvoiceDto dto);
        Task<bool> UpdateAsync(int invoiceId, UpdateInvoiceDto dto);
        Task<bool> DeleteAsync(int invoiceId);
        Task<List<UnpaidServiceDto>> GetUnpaidTestsAsync();
        Task<List<UnpaidServiceDto>> GetUnpaidAppointmentsAsync();
        Task<List<UnpaidServiceDto>> GetUnpaidPrescriptionsAsync();
    }

    public interface IUserAccountService
    {
        Task<List<UserAccount>> GetAllAsync();
        Task<UserAccount?> FindByIdAsync(Guid userId);
        Task<UserAccount?> FindByEmailAsync(string email);
        Task<bool> DeleteAsync(Guid userId);
        Task<bool> UpdateAsync(UserAccount user);
        Task<bool> ChangePasswordAsync(UserAccount user, string currentPassword, string newPassword);
        Task<List<UserAccountDetailsDto>> GetAllUserAccountsDetailedAsync();
        Task<List<PatientWithoutAccountDto>> GetPatientsWithoutAccountAsync();
        Task<List<DoctorWithoutAccountDto>> GetDoctorsWithoutAccountAsync();
        Task<List<PharmacistWithoutAccountDto>> GetPharmacistsWithoutAccountAsync();
        Task<List<StaffWithoutAccountDto>> GetStaffWithoutAccountAsync();
    }
}
