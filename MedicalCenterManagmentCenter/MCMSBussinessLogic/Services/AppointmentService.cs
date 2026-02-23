using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentData _appointmentData;

        public AppointmentService(IAppointmentData appointmentData)
        {
            _appointmentData = appointmentData;
        }

        public Task<bool> AddAsync(Appointment appointment)
        {
            return _appointmentData.InsertAppointmentAsync(appointment.DTO);
        }

        public Task<bool> UpdateAsync(Appointment appointment)
        {
            return _appointmentData.UpdateAppointmentAsync(appointment.DTO);
        }

        public Task<bool> DeleteAsync(int appointmentId)
        {
            return _appointmentData.DeleteAppointmentAsync(appointmentId);
        }

        public async Task<Appointment?> FindByIdAsync(int appointmentId)
        {
            var dto = await _appointmentData.GetAppointmentByIdAsync(appointmentId);
            return dto != null ? new Appointment(dto) : null;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            var dtos = await _appointmentData.GetAllAppointmentsAsync();
            return dtos.Select(dto => new Appointment(dto)).ToList();
        }

        public Task<List<AppointmentPatientDto>> GetByPatientIdAsync(Guid patientId)
        {
            return _appointmentData.GetAppointmentsByPatientIdAsync(patientId);
        }

        public Task<List<AppointmentSummaryDto>> GetAppointmentsWithDetailsAsync()
        {
            return _appointmentData.GetAppointmentsWithDetailsAsync();
        }
    }
}
