using MCMSDAL;


namespace MCMSBussinessLogic
{
    public class Pharmacist : Person, IPharmacist
    {
        private static readonly PharmacistData _pharmacistData = new();
        private static readonly PersonData _personData = new();

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
            bool isPharmacist = await _pharmacistData.IsPharmacistExistsByNameAsync(FirstName, SecondName, ThirdName);
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
                Guid personId = await _personData.AddPersonAsync(this.PDTO);
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

            this.PharmacistId = await _pharmacistData.CreatePharmacistAsync(pharmacistDto);
            return this.PharmacistId != Guid.Empty;
        }

        public async Task<bool> UpdatePharmacistAsync()
        {
            if (!await _personData.IsPersonExistsByIdAsync(PersonId) || !await _pharmacistData.IsPharmacistExistsByIdAsync(PharmacistId))
                return false;

            bool personUpdated = await _personData.UpdatePersonAsync(PDTO);
            bool pharmacistUpdated = await _pharmacistData.UpdatePharmacistAsync(DTO);
            return personUpdated && pharmacistUpdated;
        }

        public static async Task<bool> DeletePharmacistByIdAsync(Guid PharmacistID, Guid PersonID)
        {
            
            bool isPatient = await Patient.IsPatientExistsByPersonIdAsync(PersonID);
            bool PharmacistDeleted = await _pharmacistData.DeletePharmacistAsync(PharmacistID);
            if ( !isPatient && PharmacistDeleted) 
            {
                await DeletePersonByIdAsync(PersonID);
            }

            return PharmacistDeleted;
        }

        public static async Task<Pharmacist?> FindPharmacistByIdAsync(Guid pharmacistId)
        {
            var dto = await _pharmacistData.GetPharmacistByIdAsync(pharmacistId);
            return dto != null ? new Pharmacist(dto) : null;
        }

        public static async Task<bool> IsPharmacistExistsByIdAsync(Guid pharmacistId)
        {
            return await _pharmacistData.IsPharmacistExistsByIdAsync(pharmacistId);
        }

        public static async Task<bool> IsPharmacistExistsByNameAsync(string firstName, string secondName, string? thirdName = null)
        {
            return await _pharmacistData.IsPharmacistExistsByNameAsync(firstName, secondName, thirdName);
        }

        public static async Task<bool> IsPharmacistExistsByPersonIdAsync(Guid personId)
        {
            return await _pharmacistData.IsPharmacistExistsByPersonIdAsync(personId);
        }
        public static async Task<List<Pharmacist>> GetAllPharmacistsAsync ()
        {
            // Fetch doctor data from the database
            var PharmacistDTOs = await _pharmacistData.GetAllPharmacistsAsync();

            // Map the DTOs to domain models
            var Pharmacists = PharmacistDTOs.Select(dto => new Pharmacist(dto)).ToList();

            return Pharmacists;
        }

        public static async Task<List<PharmacistSummaryDto>> GetPharmacistSummariesAsync()
        {
            var pharmacistDTOs = await _pharmacistData.GetAllPharmacistsAsync();

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
            return await _pharmacistData.GetPharmacyDashboardStatsAsync();
        }



    }

}
