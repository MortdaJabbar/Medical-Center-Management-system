using MCMSDAL;


namespace MCMSBussinessLogic
{
    public class Pharmacist : Person
    {
        public Guid PharmacistId { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly HireDate { get; set; }
        public int? ExpereinceYears { get; set; }
        public PharmacistDTO DTO => new PharmacistDTO
        {
            PharmacistId = this.PharmacistId,
            LicenseNumber = this.LicenseNumber,
            HireDate = this.HireDate,
            ExpereinceYears = this.ExpereinceYears,
            Person = this.PDTO

        };
        public Pharmacist() { }
        public Pharmacist(PharmacistDTO dto) : base(dto.Person)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            PharmacistId = dto.PharmacistId;
            LicenseNumber = dto.LicenseNumber;
            HireDate = dto.HireDate;
            ExpereinceYears = dto.ExpereinceYears;
        }

        public async Task<bool> AddNewPharmacistAsync()
        {
            bool isPharmacist = await PharmacistData.IsPharmacistExistsByNameAsync(FirstName, SecondName, ThirdName);
            bool isDoctor = await Doctor.IsDoctorExistsByNameAsync(FirstName, SecondName, ThirdName);

            if (isPharmacist || isDoctor)
                return false;

            Person? person = await Person.FindPersonByNameAsync(FirstName, SecondName, ThirdName);

            if (person != null)
            {
                this.PersonId = person.PersonId;
            }
            else
            {
                Guid personId = await PersonData.AddPersonAsync(this.PDTO);
                if (personId == Guid.Empty)
                    return false;

                this.PersonId = personId;
            }

            var pharmacistDto = new PharmacistDTO
            {
                Person = this.PDTO,
                LicenseNumber = this.LicenseNumber,
                HireDate = this.HireDate,
                ExpereinceYears = this.ExpereinceYears
                
            };

            this.PharmacistId = await PharmacistData.CreatePharmacistAsync(pharmacistDto);
            return this.PharmacistId != Guid.Empty;
        }

        public async Task<bool> UpdatePharmacistAsync()
        {
            if (!await PersonData.IsPersonExistsByIdAsync(PersonId) || !await PharmacistData.IsPharmacistExistsByIdAsync(PharmacistId))
                return false;

            bool personUpdated = await PersonData.UpdatePersonAsync(PDTO);
            bool pharmacistUpdated = await PharmacistData.UpdatePharmacistAsync(DTO);
            return personUpdated && pharmacistUpdated;
        }

        public static async Task<bool> DeletePharmacistByIdAsync(Guid PharmacistID, Guid PersonID)
        {
            
            bool isPatient = await Patient.IsPatientExistsByPersonIdAsync(PersonID);
            bool PharmacistDeleted = await PharmacistData.DeletePharmacistAsync(PharmacistID);
            if ( !isPatient && PharmacistDeleted) 
            {
                await DeletePersonByIdAsync(PersonID);
            }

            return PharmacistDeleted;
        }

        public static async Task<Pharmacist?> FindPharmacistByIdAsync(Guid pharmacistId)
        {
            var dto = await PharmacistData.GetPharmacistByIdAsync(pharmacistId);
            return dto != null ? new Pharmacist(dto) : null;
        }

        public static async Task<bool> IsPharmacistExistsByIdAsync(Guid pharmacistId)
        {
            return await PharmacistData.IsPharmacistExistsByIdAsync(pharmacistId);
        }

        public static async Task<bool> IsPharmacistExistsByNameAsync(string firstName, string secondName, string? thirdName = null)
        {
            return await PharmacistData.IsPharmacistExistsByNameAsync(firstName, secondName, thirdName);
        }

        public static async Task<bool> IsPharmacistExistsByPersonIdAsync(Guid personId)
        {
            return await PharmacistData.IsPharmacistExistsByPersonIdAsync(personId);
        }
        public static async Task<List<Pharmacist>> GetAllPharmacistsAsync ()
        {
            // Fetch doctor data from the database
            var PharmacistDTOs = await PharmacistData.GetAllPharmacistsAsync();

            // Map the DTOs to domain models
            var Pharmacists = PharmacistDTOs.Select(dto => new Pharmacist(dto)).ToList();

            return Pharmacists;
        }

        public static async Task<List<PharmacistSummaryDto>> GetPharmacistSummariesAsync()
        {
            var pharmacistDTOs = await PharmacistData.GetAllPharmacistsAsync();

            var summaries = pharmacistDTOs.Select(dto => new PharmacistSummaryDto
            {
                PharmacistId = dto.PharmacistId,
                FullName = $"{dto.Person.FirstName} {dto.Person.SecondName} {dto.Person.ThirdName}",
                ImagePath = dto.Person.ImageLocation
            }).ToList();

            return summaries;
        }

        public static async Task<PharmacyDashboardStatsDto> GetPharmacyDashboardStatsAsync()
        {
            return await PharmacistData.GetPharmacyDashboardStatsAsync();
        }



    }

}
