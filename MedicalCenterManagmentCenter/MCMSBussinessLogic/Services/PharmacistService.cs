using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class PharmacistService : IPharmacistService
    {
        private readonly IPharmacistData _pharmacistData;
        private readonly IPersonData _personData;
        private readonly IDoctorData _doctorData;
        private readonly IPatientData _patientData;

        public PharmacistService(
            IPharmacistData pharmacistData,
            IPersonData personData,
            IDoctorData doctorData,
            IPatientData patientData)
        {
            _pharmacistData = pharmacistData;
            _personData = personData;
            _doctorData = doctorData;
            _patientData = patientData;
        }

        public async Task<Guid?> CreateAsync(Pharmacist pharmacist)
        {
            var isPharmacist = await _pharmacistData.IsPharmacistExistsByNameAsync(pharmacist.FirstName, pharmacist.SecondName, pharmacist.ThirdName);
            var isDoctor = await _doctorData.IsDoctorExistsByNameAsync(pharmacist.FirstName, pharmacist.SecondName, pharmacist.ThirdName);

            if (isPharmacist || isDoctor)
                return null;

            var existingPerson = await _personData.GetPersonByNameAsync(pharmacist.FirstName, pharmacist.SecondName, pharmacist.ThirdName);
            if (existingPerson != null)
            {
                pharmacist.PersonId = existingPerson.PersonId;
            }
            else
            {
                var personId = await _personData.AddPersonAsync(pharmacist.PDTO);
                if (personId == Guid.Empty)
                    return null;
                pharmacist.PersonId = personId;
            }

            var dto = new PharmacistDTO
            {
                Person = pharmacist.PDTO,
                LicenseNumber = pharmacist.LicenseNumber,
                HireDate = pharmacist.HireDate,
                ExpereinceYears = pharmacist.ExpereinceYears
            };

            var pharmacistId = await _pharmacistData.CreatePharmacistAsync(dto);
            if (pharmacistId == Guid.Empty)
                return null;
            pharmacist.PharmacistId = pharmacistId;
            return pharmacistId;
        }

        public async Task<bool> UpdateAsync(Pharmacist pharmacist)
        {
            if (!await _personData.IsPersonExistsByIdAsync(pharmacist.PersonId) || !await _pharmacistData.IsPharmacistExistsByIdAsync(pharmacist.PharmacistId))
                return false;

            var personUpdated = await _personData.UpdatePersonAsync(pharmacist.PDTO);
            var pharmacistUpdated = await _pharmacistData.UpdatePharmacistAsync(pharmacist.DTO);
            return personUpdated && pharmacistUpdated;
        }

        public async Task<Pharmacist?> FindByIdAsync(Guid pharmacistId)
        {
            var dto = await _pharmacistData.GetPharmacistByIdAsync(pharmacistId);
            return dto != null ? new Pharmacist(dto) : null;
        }

        public async Task<bool> DeleteAsync(Guid pharmacistId, Guid personId)
        {
            var deleted = await _pharmacistData.DeletePharmacistAsync(pharmacistId);
            if (!deleted)
                return false;

            var isPatient = await _patientData.IsPatientExistsByPersonIdAsync(personId);
            if (!isPatient)
                await _personData.DeletePersonAsync(personId);

            return true;
        }

        public async Task<List<Pharmacist>> GetAllAsync()
        {
            var dtos = await _pharmacistData.GetAllPharmacistsAsync();
            return dtos.Select(dto => new Pharmacist(dto)).ToList();
        }

        public Task<PharmacyDashboardStatsDto> GetPharmacyDashboardStatsAsync()
        {
            return _pharmacistData.GetPharmacyDashboardStatsAsync();
        }
    }
}
