using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionData _prescriptionData;

        public PrescriptionService(IPrescriptionData prescriptionData)
        {
            _prescriptionData = prescriptionData;
        }

        public async Task<int?> CreateAsync(Prescription prescription)
        {
            var newId = await _prescriptionData.CreatePrescriptionAsync(prescription.DTO);
            if (newId <= 0)
                return null;
            prescription.PrescriptionID = newId;
            return newId;
        }

        public Task<bool> UpdateAsync(Prescription prescription)
        {
            return _prescriptionData.UpdatePrescriptionAsync(prescription.DTO);
        }

        public async Task<Prescription?> FindByIdAsync(int prescriptionId)
        {
            var dto = await _prescriptionData.GetPrescriptionByIdAsync(prescriptionId);
            return dto != null ? new Prescription(dto) : null;
        }

        public async Task<List<Prescription>> GetAllAsync()
        {
            var dtos = await _prescriptionData.GetAllPrescriptionsAsync();
            return dtos.Select(dto => new Prescription(dto)).ToList();
        }

        public async Task<List<Prescription>> GetPagedAsync(int page, int size)
        {
            var dtos = await _prescriptionData.GetPagedPrescriptionsAsync(page, size);
            return dtos.Select(dto => new Prescription(dto)).ToList();
        }

        public Task<bool> DeleteAsync(int prescriptionId)
        {
            return _prescriptionData.DeletePrescriptionAsync(prescriptionId);
        }

        public Task<List<PrescriptionDetailsDto>> GetDetailedAsync()
        {
            return _prescriptionData.GetAllWithNamesAsync();
        }
    }
}
