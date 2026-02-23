using MCMSDAL;
using MCMSDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
    public class Appointment : IAppointment
    {
        private readonly IAppointmentData _appointmentData;

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

        public Appointment() : this(new AppointmentData()) { }

        public Appointment(IAppointmentData appointmentData)
        {
            _appointmentData = appointmentData;
        }

        public Appointment(AppointmentDto dto) : this(new AppointmentData())
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
            return await _appointmentData.InsertAppointmentAsync(this.DTO);
        }

        public async Task<bool> UpdateAppointmentAsync()
        {
            return await _appointmentData.UpdateAppointmentAsync(this.DTO);
        }

        public static async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointmentData = new AppointmentData();
            return await appointmentData.DeleteAppointmentAsync(id);
        }

        public static async Task<Appointment?> FindByIdAsync(int id)
        {
            var appointmentData = new AppointmentData();
            var dto = await appointmentData.GetAppointmentByIdAsync(id);
            return dto != null ? new Appointment(dto) : null;
        }

        public static async Task<List<Appointment>> GetAllAsync()
        {
            var appointmentData = new AppointmentData();
            var dtos = await appointmentData.GetAllAppointmentsAsync();
            return dtos.Select(dto => new Appointment(dto)).ToList();
        }



        public static async Task<List<AppointmentPatientDto>> GetByPatientIdAsync(Guid patientId)
        {
            var appointmentData = new AppointmentData();
            return await appointmentData.GetAppointmentsByPatientIdAsync(patientId);
        }

      
        public static async Task<List<AppointmentSummaryDto>> GetAppointmentsWithDetailsAsync()
        {
            var appointmentData = new AppointmentData();
            return await appointmentData.GetAppointmentsWithDetailsAsync();
        }


    }

}
