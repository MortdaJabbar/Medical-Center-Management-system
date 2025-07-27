using MCMSDAL;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Staff : Person
    {
        public Guid StaffId { get; set; }
        public DateOnly HireDate { get; set; }
        public bool IsAdmin { get; set; }

        public StaffDTO DTO => new()
        {
            StaffId = this.StaffId,
            HireDate = this.HireDate,
            IsAdmin = this.IsAdmin,
            Person = this.PDTO
        };

        public Staff() { }

        public Staff(StaffDTO dto) : base(dto.Person)
        {
            StaffId = dto.StaffId;
            HireDate = dto.HireDate;
            IsAdmin = dto.IsAdmin;
        }

        public async Task<bool> AddNewStaffAsync()
        {
            var existingPerson = await Person.FindPersonByNameAsync(FirstName, SecondName, ThirdName);
            if (existingPerson != null)
            {
                PersonId = existingPerson.PersonId;
            }
            else
            {
                var newPersonId = await PersonData.AddPersonAsync(PDTO);
                if (newPersonId == Guid.Empty) return false;
                PersonId = newPersonId;
            }

            var dto = this.DTO;
            StaffId = await StaffData.InsertStaffAsync(dto);
            return StaffId != Guid.Empty;
        }

        public async Task<bool> UpdateStaffAsync()
        {
            bool personUpdated = await PersonData.UpdatePersonAsync(this.PDTO);
            bool staffUpdated = await StaffData.UpdateStaffAsync(this.StaffId, this.DTO);
            return personUpdated && staffUpdated;
        }

        public static async Task<Staff?> FindStaffByIdAsync(Guid id)
        {
            var dto = await StaffData.GetStaffByIdAsync(id);
            return dto != null ? new Staff(dto) : null;
        }

        public static async Task<bool> DeleteStaffByIdAsync(Guid staffId,Guid PersonID)
        {
           
            bool isPatient = await Patient.IsPatientExistsByPersonIdAsync(PersonID);
           
            bool StaffDeleted = await StaffData.DeleteStaffAsync(staffId);
            if (  !isPatient    && StaffDeleted)
            {
                await DeletePersonByIdAsync(PersonID);
            }

            return StaffDeleted;
        }

        public static async Task<List<Staff>> GetAllStaffAsync()
        {
            var dtos = await StaffData.GetAllStaffAsync();
            return dtos.Select(dto => new Staff(dto)).ToList();
        }

        public static async Task<List<StaffSummaryDto>> GetStaffSummariesAsync()
        {
            return await StaffData.GetAllStaffSummariesAsync();
        }

        public static async Task<StaffDashboardStatsDto> GetDashboardStatsAsync()
        {
            return await StaffData.GetStaffDashboardStatsAsync();
        }


        public static async Task<AdminDashboardStatsDto> GetAdminDashboardStatsAsync()
        {
            return await StaffData.GetAdminDashboardStatsAsync();
        }

    }

}
