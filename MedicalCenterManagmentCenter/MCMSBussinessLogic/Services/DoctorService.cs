using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorData _doctorData;
        private readonly IPersonData _personData;
        private readonly IPharmacistData _pharmacistData;
        private readonly IPatientData _patientData;
        private readonly IAppointmentData _appointmentData;
        private readonly ITestData _testData;
        private readonly IPrescriptionData _prescriptionData;

        public DoctorService(
            IDoctorData doctorData,
            IPersonData personData,
            IPharmacistData pharmacistData,
            IPatientData patientData,
            IAppointmentData appointmentData,
            ITestData testData,
            IPrescriptionData prescriptionData)
        {
            _doctorData = doctorData;
            _personData = personData;
            _pharmacistData = pharmacistData;
            _patientData = patientData;
            _appointmentData = appointmentData;
            _testData = testData;
            _prescriptionData = prescriptionData;
        }

        public async Task<Guid?> CreateAsync(Doctor doctor)
        {
            var isDoctor = await _doctorData.IsDoctorExistsByNameAsync(doctor.FirstName, doctor.SecondName, doctor.ThirdName);
            var isPharmacist = await _pharmacistData.IsPharmacistExistsByNameAsync(doctor.FirstName, doctor.SecondName, doctor.ThirdName);
            if (isDoctor || isPharmacist)
                return null;

            var existingPerson = await _personData.GetPersonByNameAsync(doctor.FirstName, doctor.SecondName, doctor.ThirdName);
            if (existingPerson != null)
            {
                doctor.PersonId = existingPerson.PersonId;
            }
            else
            {
                var personId = await _personData.AddPersonAsync(doctor.PDTO);
                if (personId == Guid.Empty)
                    return null;
                doctor.PersonId = personId;
            }

            var newDoctorDto = new DoctorDTO
            {
                Person = doctor.PDTO,
                Specialization = doctor.Specialization,
                Available = doctor.Available,
                ScheduleId = doctor.ScheduleId,
                Experienceyears = doctor.Experienceyears
            };

            var doctorId = await _doctorData.AddNewDoctorAsync(newDoctorDto);
            if (doctorId == Guid.Empty)
                return null;

            doctor.DoctorId = doctorId;
            return doctorId;
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            if (!await _personData.IsPersonExistsByIdAsync(doctor.PersonId) || !await _doctorData.IsDoctorExistsByIdAsync(doctor.DoctorId))
                return false;

            var personUpdated = await _personData.UpdatePersonAsync(doctor.PDTO);
            var doctorUpdated = await _doctorData.UpdateDoctorAsync(doctor.DTO);

            return personUpdated && doctorUpdated;
        }

        public async Task<Doctor?> FindByIdAsync(Guid doctorId)
        {
            var dto = await _doctorData.GetDoctorByIdAsync(doctorId);
            return dto != null ? new Doctor(dto) : null;
        }

        public async Task<bool> DeleteAsync(Guid doctorId, Guid personId)
        {
            var deleted = await _doctorData.DeleteDoctorAsync(doctorId);
            if (!deleted)
                return false;

            var isPatient = await _patientData.IsPatientExistsByPersonIdAsync(personId);
            if (!isPatient)
                await _personData.DeletePersonAsync(personId);

            return true;
        }

        public async Task<List<Doctor>> GetAllAsync()
        {
            var dtos = await _doctorData.GetAllDoctorsAsync();
            return dtos.Select(dto => new Doctor(dto)).ToList();
        }

        public async Task<List<DoctorSummaryDto>> GetSummariesAsync()
        {
            var dtos = await _doctorData.GetAllDoctorsAsync();

            return dtos.Select(dto => new DoctorSummaryDto
            {
                DoctorId = dto.DoctorId,
                FullName = $"{dto.Person.FirstName} {dto.Person.SecondName} {dto.Person.ThirdName}",
                ImagePath = dto.Person.ImageLocation
            }).ToList();
        }

        public Task<List<AppointmentByDoctorDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return _appointmentData.GetAppointmentsByDoctorIdAsync(doctorId);
        }

        public Task<List<TestDoctorDto>> GetTestsByDoctorIdAsync(Guid doctorId)
        {
            return _testData.GetTestsByDoctorIdAsync(doctorId);
        }

        public Task<List<PrescriptionByDoctorDto>> GetPrescriptionsByDoctorIdAsync(Guid doctorId)
        {
            return _prescriptionData.GetPrescriptionsByDoctorIdAsync(doctorId);
        }

        public Task<DoctorDashboardStatsDto?> GetDashboardStatsAsync(Guid doctorId)
        {
            return _doctorData.GetDoctorDashboardStatsAsync(doctorId);
        }
    }
}
