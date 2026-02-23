using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IPrescriptionData
    {
        Task<int> CreatePrescriptionAsync(PrescriptionDto dto);
        Task<bool> UpdatePrescriptionAsync(PrescriptionDto dto);
        Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id);
        Task<List<PrescriptionDto>> GetAllPrescriptionsAsync();
        Task<List<PrescriptionDto>> GetPagedPrescriptionsAsync(int pageNumber, int pageSize);
        Task<bool> DeletePrescriptionAsync(int id);
        Task<List<PrescriptionPatientDto>> GetPrescriptionsByPatientIdAsync(Guid patientId);
        Task<List<PrescriptionDetailsDto>> GetAllWithNamesAsync();
        Task<List<PrescriptionByDoctorDto>> GetPrescriptionsByDoctorIdAsync(Guid doctorId);
    }
}
