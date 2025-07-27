using MCMSDAL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MCMSBussinessLogic
{
    public class Patient : Person
    {
        public PatientDTO DTO
        {
            get
            {
                return new PatientDTO
                {
                    PatientId = PatientId,
                    Weight = Weight,
                    Height = Height,
                    Person = this.PDTO
                };
            }
        }
        public Guid PatientId { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public Patient() : base()
        {
           
            PatientId =Guid.Empty;
            Weight = 0;
            Height = 0;
        }
        public Patient(PatientDTO dto) : base(dto.Person)
        {
            if (dto == null)  throw new ArgumentNullException(nameof(dto));
            PatientId = dto.PatientId;
            Weight = dto.Weight;
            Height = dto.Height;
        }

        public async Task<bool> AddNewPatientAsync()
        {
            if (await Patient.IsPatientExistsByNameAsync(FirstName, SecondName, ThirdName))
                return false;


            Person ?person =  await Person.FindPersonByNameAsync(FirstName, SecondName, ThirdName) ;
            if (person != null)
            {
                this.PersonId = person.PersonId;
            }

            else
            {
                Guid personId = await PersonData.AddPersonAsync(this.PDTO);
                if (personId == Guid.Empty) return false;
                this.PersonId = personId;
            }


          

            // Add Patient
            var newPatientDto = new PatientDTO
            {
                Person = this.PDTO,
                Weight = this.Weight,
                Height = this.Height
            };

            this.PatientId = await PatientData.CreatePatientAsync(newPatientDto);
            return this.PatientId != Guid.Empty;
        }
        public async Task<bool> UpdatePatientAsync()
        {
            if (!await PersonData.IsPersonExistsByIdAsync(PersonId) || !await PatientData.IsPatientExistsByIdAsync(PatientId))
                return false;

            bool personUpdated = await PersonData.UpdatePersonAsync(PDTO);
            bool patientUpdated = await PatientData.UpdatePatientAsync(DTO);
            return personUpdated && patientUpdated;
        }
        public static async Task<Patient?> FindPatientByIdAsync(Guid patientId)
        {
            var dto = await PatientData.GetPatientByIdAsync(patientId);
            return (dto != null) ? new Patient(dto) : null;
        }
        public static  async Task<bool> DeletePatientByIdAsync(Guid PatientId ,Guid PersonID)
        {
            bool isDoctor = await Doctor.IsDoctorExistsByPersonIdAsync(PersonID);
            bool IsPharmacist = await PharmacistData.IsPharmacistExistsByPersonIdAsync(PersonID);
            bool PatientDeleted = await PatientData.DeletePatientAsync(PatientId);

            if (!isDoctor && !IsPharmacist && PatientDeleted)
            {
                await DeletePersonByIdAsync(PersonID);
            }

            return PatientDeleted;

        }
        public static async Task<bool> IsPatientExistsByNameAsync(string firstName, string secondName, string? thirdName = null) 
        {
            return await PatientData.IsPatientExistsByNameAsync(firstName, secondName, thirdName);
        }
        public static async Task<bool> IsPatientExistsByIdAsync(Guid PatientId)
        {
            return await PatientData.IsPatientExistsByIdAsync(PatientId);
        }
        public static async Task<bool> IsPatientExistsByPersonIdAsync(Guid personId)
        {
            return await PatientData.IsPatientExistsByPersonIdAsync(personId);
        }
        public static async Task<List<Patient>> GetAllPatientsAsync( )
        {
            // Fetch doctor data from the database
            var doctorDTOs = await PatientData.GetAllPatientsAsync ();

            // Map the DTOs to domain models
            var doctors = doctorDTOs.Select(dto => new Patient(dto)).ToList();

            return doctors;
        }
        public static async Task<List<PatientSummaryDto>> GetPatientSummariesAsync()
        {
            var patientDTOs = await PatientData.GetAllPatientsAsync();

            var summaries = patientDTOs.Select(dto => new PatientSummaryDto
            {
                PatientId = dto.PatientId,
                FullName = $"{dto.Person.FirstName} {dto.Person.SecondName} {dto.Person.ThirdName}",
                ImagePath = dto.Person.ImageLocation
            }).ToList();

            return summaries;
        }
        public static async Task<List<AppointmentPatientDto>> GetPatientAppoitments(Guid id) 
        {
            return await  AppointmentData.GetAppointmentsByPatientIdAsync(id);
        }
        public static async Task<List<PrescriptionPatientDto>> GetPrescriptionsByPatientIdAsync(Guid patientId)
        {
            return await  PrescriptionData.GetPrescriptionsByPatientIdAsync(patientId);
        }
        public static async Task<List<TestPatientsDto>> GetTestsByPatientIdAsync(Guid patientId) 
        {
            return await TestData.GetTestsByPatientIdAsync(patientId);
        
        }
        public static async Task<PatientDashboardDto?> GetPatientDashboardStatsAsync(Guid patientId)
        {
            return await PatientData.GetPatientDashboardStatsAsync(patientId);
        }

        public static  async Task<List<PatientInvoiceDto>> GetInvoicesForPatientAsync(Guid patientId)
        {
            return await InvoiceData.GetInvoicesForPatientAsync(patientId);
        }
    }

    }



