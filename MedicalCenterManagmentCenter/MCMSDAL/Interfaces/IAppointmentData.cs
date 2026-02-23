using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IAppointmentData
    {
        Task<bool> InsertAppointmentAsync(AppointmentDto dto);
        Task<bool> UpdateAppointmentAsync(AppointmentDto dto);
        Task<AppointmentDto?> GetAppointmentByIdAsync(int id);
        Task<List<AppointmentDto>> GetAllAppointmentsAsync();
        Task<bool> DeleteAppointmentAsync(int id);
        Task<List<AppointmentPatientDto>> GetAppointmentsByPatientIdAsync(Guid patientId);
        Task<List<AppointmentByDoctorDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<List<AppointmentSummaryDto>> GetAppointmentsWithDetailsAsync();
    }
}
