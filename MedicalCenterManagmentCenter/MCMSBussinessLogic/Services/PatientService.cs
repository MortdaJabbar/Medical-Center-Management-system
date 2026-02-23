using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientData _patientData;
        private readonly IPersonData _personData;
        private readonly IDoctorData _doctorData;
        private readonly IPharmacistData _pharmacistData;
        private readonly IAppointmentData _appointmentData;
        private readonly IPrescriptionData _prescriptionData;
        private readonly ITestData _testData;
        private readonly IInvoiceData _invoiceData;

        public PatientService(
            IPatientData patientData,
            IPersonData personData,
            IDoctorData doctorData,
            IPharmacistData pharmacistData,
            IAppointmentData appointmentData,
            IPrescriptionData prescriptionData,
            ITestData testData,
            IInvoiceData invoiceData)
        {
            _patientData = patientData;
            _personData = personData;
            _doctorData = doctorData;
            _pharmacistData = pharmacistData;
            _appointmentData = appointmentData;
            _prescriptionData = prescriptionData;
            _testData = testData;
            _invoiceData = invoiceData;
        }

        public async Task<Guid?> CreateAsync(Patient patient)
        {
            if (await _patientData.IsPatientExistsByNameAsync(patient.FirstName, patient.SecondName, patient.ThirdName))
                return null;

            var existingPerson = await _personData.GetPersonByNameAsync(patient.FirstName, patient.SecondName, patient.ThirdName);
            if (existingPerson != null)
            {
                patient.PersonId = existingPerson.PersonId;
            }
            else
            {
                var personId = await _personData.AddPersonAsync(patient.PDTO);
                if (personId == Guid.Empty)
                    return null;
                patient.PersonId = personId;
            }

            var newPatientDto = new PatientDTO
            {
                Person = patient.PDTO,
                Weight = patient.Weight,
                Height = patient.Height
            };

            var patientId = await _patientData.CreatePatientAsync(newPatientDto);
            if (patientId == Guid.Empty)
                return null;
            patient.PatientId = patientId;
            return patientId;
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            if (!await _personData.IsPersonExistsByIdAsync(patient.PersonId) || !await _patientData.IsPatientExistsByIdAsync(patient.PatientId))
                return false;

            var personUpdated = await _personData.UpdatePersonAsync(patient.PDTO);
            var patientUpdated = await _patientData.UpdatePatientAsync(patient.DTO);
            return personUpdated && patientUpdated;
        }

        public async Task<Patient?> FindByIdAsync(Guid patientId)
        {
            var dto = await _patientData.GetPatientByIdAsync(patientId);
            return dto != null ? new Patient(dto) : null;
        }

        public async Task<bool> DeleteAsync(Guid patientId, Guid personId)
        {
            var deleted = await _patientData.DeletePatientAsync(patientId);
            if (!deleted)
                return false;

            var isDoctor = await _doctorData.IsDoctorExistsByPersonIdAsync(personId);
            var isPharmacist = await _pharmacistData.IsPharmacistExistsByPersonIdAsync(personId);

            if (!isDoctor && !isPharmacist)
                await _personData.DeletePersonAsync(personId);

            return true;
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            var dtos = await _patientData.GetAllPatientsAsync();
            return dtos.Select(dto => new Patient(dto)).ToList();
        }

        public async Task<List<PatientSummaryDto>> GetSummariesAsync()
        {
            var dtos = await _patientData.GetAllPatientsAsync();
            return dtos.Select(dto => new PatientSummaryDto
            {
                PatientId = dto.PatientId,
                FullName = $"{dto.Person.FirstName} {dto.Person.SecondName} {dto.Person.ThirdName}",
                ImagePath = dto.Person.ImageLocation
            }).ToList();
        }

        public Task<List<AppointmentPatientDto>> GetAppointmentsAsync(Guid patientId)
        {
            return _appointmentData.GetAppointmentsByPatientIdAsync(patientId);
        }

        public Task<List<PrescriptionPatientDto>> GetPrescriptionsAsync(Guid patientId)
        {
            return _prescriptionData.GetPrescriptionsByPatientIdAsync(patientId);
        }

        public Task<List<TestPatientsDto>> GetTestsAsync(Guid patientId)
        {
            return _testData.GetTestsByPatientIdAsync(patientId);
        }

        public Task<PatientDashboardDto?> GetDashboardStatsAsync(Guid patientId)
        {
            return _patientData.GetPatientDashboardStatsAsync(patientId);
        }

        public Task<List<PatientInvoiceDto>> GetInvoicesAsync(Guid patientId)
        {
            return _invoiceData.GetInvoicesForPatientAsync(patientId);
        }
    }
}
