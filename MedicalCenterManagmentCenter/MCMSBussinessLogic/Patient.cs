using MCMSDAL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MCMSBussinessLogic
{
    public class Patient : Person, IPatient
    {
        private static readonly PersonData _personData = new();
        private static readonly PatientData _patientData = new();
        private static readonly PharmacistData _pharmacistData = new();
        private static readonly AppointmentData _appointmentData = new();
        private static readonly PrescriptionData _prescriptionData = new();
        private static readonly TestData _testData = new();
        private static readonly InvoiceData _invoiceData = new();

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
                Guid personId = await _personData.AddPersonAsync(this.PDTO);
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

            this.PatientId = await _patientData.CreatePatientAsync(newPatientDto);
            return this.PatientId != Guid.Empty;
        }
        public async Task<bool> UpdatePatientAsync()
        {
            if (!await _personData.IsPersonExistsByIdAsync(PersonId) || !await _patientData.IsPatientExistsByIdAsync(PatientId))
                return false;

            bool personUpdated = await _personData.UpdatePersonAsync(PDTO);
            bool patientUpdated = await _patientData.UpdatePatientAsync(DTO);
            return personUpdated && patientUpdated;
        }
        public static async Task<Patient?> FindPatientByIdAsync(Guid patientId)
        {
            var dto = await _patientData.GetPatientByIdAsync(patientId);
            return (dto != null) ? new Patient(dto) : null;
        }
        public static  async Task<bool> DeletePatientByIdAsync(Guid PatientId ,Guid PersonID)
        {
            bool isDoctor = await Doctor.IsDoctorExistsByPersonIdAsync(PersonID);
            bool IsPharmacist = await _pharmacistData.IsPharmacistExistsByPersonIdAsync(PersonID);
            bool PatientDeleted = await _patientData.DeletePatientAsync(PatientId);

            if (!isDoctor && !IsPharmacist && PatientDeleted)
            {
                await DeletePersonByIdAsync(PersonID);
            }

            return PatientDeleted;

        }
        public static async Task<bool> IsPatientExistsByNameAsync(string firstName, string secondName, string? thirdName = null) 
        {
            return await _patientData.IsPatientExistsByNameAsync(firstName, secondName, thirdName);
        }
        public static async Task<bool> IsPatientExistsByIdAsync(Guid PatientId)
        {
            return await _patientData.IsPatientExistsByIdAsync(PatientId);
        }
        public static async Task<bool> IsPatientExistsByPersonIdAsync(Guid personId)
        {
            return await _patientData.IsPatientExistsByPersonIdAsync(personId);
        }
        public static async Task<List<Patient>> GetAllPatientsAsync( )
        {
            // Fetch doctor data from the database
            var doctorDTOs = await _patientData.GetAllPatientsAsync ();

            // Map the DTOs to domain models
            var doctors = doctorDTOs.Select(dto => new Patient(dto)).ToList();

            return doctors;
        }
        public static async Task<List<PatientSummaryDto>> GetPatientSummariesAsync()
        {
            var patientDTOs = await _patientData.GetAllPatientsAsync();

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
            return await  _appointmentData.GetAppointmentsByPatientIdAsync(id);
        }
        public static async Task<List<PrescriptionPatientDto>> GetPrescriptionsByPatientIdAsync(Guid patientId)
        {
            return await  _prescriptionData.GetPrescriptionsByPatientIdAsync(patientId);
        }
        public static async Task<List<TestPatientsDto>> GetTestsByPatientIdAsync(Guid patientId) 
        {
            return await _testData.GetTestsByPatientIdAsync(patientId);
        
        }
        public static async Task<PatientDashboardDto?> GetPatientDashboardStatsAsync(Guid patientId)
        {
            return await _patientData.GetPatientDashboardStatsAsync(patientId);
        }

        public static  async Task<List<PatientInvoiceDto>> GetInvoicesForPatientAsync(Guid patientId)
        {
            return await _invoiceData.GetInvoicesForPatientAsync(patientId);
        }
    }

    }



