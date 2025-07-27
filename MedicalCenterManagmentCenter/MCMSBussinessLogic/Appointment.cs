using MCMSDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
        public bool Paid { get; set; }
        public Patient  Patient { get; set; } = null;
        public Doctor Doctor { get; set; } = null;

        public AppointmentDto DTO => new AppointmentDto
        {
            AppointmentID = AppointmentID,
            PatientID = PatientID,
            DoctorID = DoctorID,
            AppointmentDate = AppointmentDate,
            Reason = Reason,
            Status = Status,
            Notes = Notes,
            AppointmentTime= AppointmentTime,
            Paid = Paid
        };

        public Appointment() { }

        public Appointment(AppointmentDto dto)
        {
            AppointmentID = dto.AppointmentID;
            PatientID = dto.PatientID;
            DoctorID = dto.DoctorID;
            AppointmentDate = dto.AppointmentDate;
            Reason = dto.Reason;
            Status = dto.Status;
            Notes = dto.Notes;
            AppointmentTime = dto.AppointmentTime;
            Paid = dto.Paid;
        }

        public async Task<bool> AddNewAppointmentAsync()
        {
            return await AppointmentData.InsertAppointmentAsync(this.DTO);
        }

        public async Task<bool> UpdateAppointmentAsync()
        {
            return await AppointmentData.UpdateAppointmentAsync(this.DTO);
        }

        public static async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await AppointmentData.DeleteAppointmentAsync(id);
        }

        public static async Task<Appointment?> FindByIdAsync(int id)
        {
            var dto = await AppointmentData.GetAppointmentByIdAsync(id);
            return dto != null ? new Appointment(dto) : null;
        }

        public static async Task<List<Appointment>> GetAllAsync()
        {
            var dtos = await AppointmentData.GetAllAppointmentsAsync();
            return dtos.Select(dto => new Appointment(dto)).ToList();
        }



        public static async Task<List<AppointmentPatientDto>> GetByPatientIdAsync(Guid patientId)
        {
            return await AppointmentData.GetAppointmentsByPatientIdAsync(patientId);
        }

      
        public static async Task<List<AppointmentSummaryDto>> GetAppointmentsWithDetailsAsync()
        {
            return await AppointmentData.GetAppointmentsWithDetailsAsync();
        }


    }

}
