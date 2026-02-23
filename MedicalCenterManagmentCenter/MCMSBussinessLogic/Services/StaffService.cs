using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffData _staffData;
        private readonly IPersonData _personData;
        private readonly IPatientData _patientData;

        public StaffService(IStaffData staffData, IPersonData personData, IPatientData patientData)
        {
            _staffData = staffData;
            _personData = personData;
            _patientData = patientData;
        }

        public async Task<Guid?> CreateAsync(Staff staff)
        {
            var existingPerson = await _personData.GetPersonByNameAsync(staff.FirstName, staff.SecondName, staff.ThirdName);
            if (existingPerson != null)
            {
                staff.PersonId = existingPerson.PersonId;
            }
            else
            {
                var personId = await _personData.AddPersonAsync(staff.PDTO);
                if (personId == Guid.Empty)
                    return null;
                staff.PersonId = personId;
            }

            var staffId = await _staffData.InsertStaffAsync(staff.DTO);
            if (staffId == Guid.Empty)
                return null;

            staff.StaffId = staffId;
            return staffId;
        }

        public async Task<bool> UpdateAsync(Staff staff)
        {
            var personUpdated = await _personData.UpdatePersonAsync(staff.PDTO);
            var staffUpdated = await _staffData.UpdateStaffAsync(staff.StaffId, staff.DTO);
            return personUpdated && staffUpdated;
        }

        public async Task<Staff?> FindByIdAsync(Guid staffId)
        {
            var dto = await _staffData.GetStaffByIdAsync(staffId);
            return dto != null ? new Staff(dto) : null;
        }

        public async Task<bool> DeleteAsync(Guid staffId, Guid personId)
        {
            var deleted = await _staffData.DeleteStaffAsync(staffId);
            if (!deleted)
                return false;

            var isPatient = await _patientData.IsPatientExistsByPersonIdAsync(personId);
            if (!isPatient)
                await _personData.DeletePersonAsync(personId);

            return true;
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            var dtos = await _staffData.GetAllStaffAsync();
            return dtos.Select(dto => new Staff(dto)).ToList();
        }

        public Task<List<StaffSummaryDto>> GetSummariesAsync()
        {
            return _staffData.GetAllStaffSummariesAsync();
        }

        public Task<StaffDashboardStatsDto> GetStaffDashboardStatsAsync()
        {
            return _staffData.GetStaffDashboardStatsAsync();
        }

        public Task<AdminDashboardStatsDto> GetAdminDashboardStatsAsync()
        {
            return _staffData.GetAdminDashboardStatsAsync();
        }
    }
}
