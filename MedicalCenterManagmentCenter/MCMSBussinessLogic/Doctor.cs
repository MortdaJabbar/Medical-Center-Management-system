using MCMSDAL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Doctor : Person, IDoctor
    {
        private static readonly DoctorData _doctorData = new();
        private static readonly PersonData _personData = new();
        private static readonly PharmacistData _pharmacistData = new();
        private static readonly AppointmentData _appointmentData = new();
        private static readonly TestData _testData = new();

        public Guid DoctorId { get; set; }
        public string Specialization { get; set; }
        public bool Available { get; set; }
        public int Experienceyears { get; set; }
        public string AvailableText {get{ return (Available) ? "Available" : "Not Available"; } }
        public int? ScheduleId { get; set; }
        public DoctorDTO DTO
        {
            get
            {
                return new DoctorDTO
                {
                    DoctorId = DoctorId,
                    Specialization = Specialization,
                    Available = Available,
                    ScheduleId = ScheduleId,
                    Experienceyears = Experienceyears,
                    Person = this.PDTO
                };
            }
        }

        public Doctor() : base()
        {
 
            this.DoctorId        = Guid.Empty;
            this.Specialization  = "";
            this.Available       = false;
            this.ScheduleId      = 0;
            this.Experienceyears = 0;
        }
        public Doctor(DoctorDTO dto) : base(dto.Person)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            this.DoctorId = dto.DoctorId;
            this.Specialization = dto.Specialization;
            this.Available = dto.Available;
            this.ScheduleId = dto.ScheduleId;
            this.Experienceyears = dto.Experienceyears;
        }
        // Asynchronous method to add a new doctor
        public async Task<bool> AddNewDoctorAsync()
        {
            bool IsDoctor     = await Doctor.IsDoctorExistsByNameAsync(FirstName, SecondName, ThirdName);
            bool IsPharmacist = await _pharmacistData.IsPharmacistExistsByNameAsync(FirstName, SecondName, ThirdName);

            if (IsDoctor || IsPharmacist) return false;
             
            Person? person = await Person.FindPersonByNameAsync(FirstName, SecondName, ThirdName);
            if (person != null) 
            {
                this.PersonId = person.PersonId;
            }
            
            else
            {
                            Guid   personId = await _personData.AddPersonAsync(this.PDTO);
                if (personId == Guid.Empty) return false;
                this.PersonId = personId;
            }


            var newDoctorDto = new DoctorDTO
            {
                Person = this.PDTO,
                Specialization = this.Specialization,
                Available = this.Available,
                ScheduleId = this.ScheduleId
            };

            this.DoctorId = await _doctorData.AddNewDoctorAsync(newDoctorDto);
            return this.DoctorId != Guid.Empty;
        }
        // Asynchronous method to update a doctor's information
        public async Task<bool> UpdateDoctorAsync()
        {
            if (!await _personData.IsPersonExistsByIdAsync(PersonId) || !await _doctorData.IsDoctorExistsByIdAsync(DoctorId))
                return false;

            bool personUpdated = await _personData.UpdatePersonAsync(PDTO);
            bool doctorUpdated = await _doctorData.UpdateDoctorAsync(DTO);
            return personUpdated && doctorUpdated;
        }
        // Asynchronous method to find a doctor by their ID
        public static async Task<Doctor?> FindDoctorByIdAsync(Guid doctorId)
        {
            var dto = await _doctorData.GetDoctorByIdAsync(doctorId);
            return (dto != null) ? new Doctor(dto) : null;
        }
        // Asynchronous method to delete a doctor by their ID
        public  static  async Task<bool> DeleteDoctorByIdAsync(Guid DoctorId, Guid PersonID)
        {

            bool DoctorDeleted = await _doctorData.DeleteDoctorAsync(DoctorId); 
            
            bool isPatient = await Patient.IsPatientExistsByPersonIdAsync(PersonID);
           
            if (!isPatient && DoctorDeleted) 
            {
                await  DeletePersonByIdAsync(PersonID); 
            }

                return DoctorDeleted;
        }
        public static async Task<bool> IsDoctorExistsByIdAsync(Guid doctorId)
        {
            using var connection = new SqlConnection(AppConfig.ConnectionString);
            using var command = new SqlCommand("dbo.IsDoctorExistsById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToBoolean(result);
        }
        public static async Task<bool> IsDoctorExistsByNameAsync(string firstName, string secondName, string? thirdName = null)
        {
            return await _doctorData.IsDoctorExistsByNameAsync(firstName, secondName, thirdName);
        }
        public static async Task<bool> IsDoctorExistsByPersonIdAsync(Guid personId)
        {
            return await _doctorData.IsDoctorExistsByPersonIdAsync(personId);
        }
        public static  async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            // Fetch doctor data from the database
            var doctorDTOs = await _doctorData.GetAllDoctorsAsync( );

            // Map the DTOs to domain models
            var doctors = doctorDTOs.Select(dto => new Doctor(dto)).ToList();

            return doctors;
        }

        public static async Task<List<DoctorSummaryDto>> GetDoctorSummariesAsync()
        {
            var doctorDTOs = await _doctorData.GetAllDoctorsAsync();

            var summaries = doctorDTOs.Select(dto => new DoctorSummaryDto
            {
                DoctorId = dto.DoctorId,
                FullName = $"{dto.Person.FirstName} {dto.Person.SecondName} {dto.Person.ThirdName}",
                ImagePath = dto.Person.ImageLocation
            }).ToList();

            return summaries;
        }



        public static async Task<List<AppointmentByDoctorDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _appointmentData.GetAppointmentsByDoctorIdAsync(doctorId);
        }

        public static async Task<List<TestDoctorDto>> GetTestsByDoctorIdAsync(Guid DoctorId)
        {
            var Tests = await _testData.GetTestsByDoctorIdAsync(DoctorId);


            return Tests;
        }


        public static async Task<List<PrescriptionByDoctorDto>> GetPrescriptionsByDoctorIdAsync(Guid doctorId) 
        {
            return await Prescription.GetPrescriptionsByDoctorIdAsync(doctorId);
        }

        public static async Task<DoctorDashboardStatsDto?> GetDashboardStatsAsync(Guid doctorId)
        {
            return await _doctorData.GetDoctorDashboardStatsAsync(doctorId);
        }

    }

}
