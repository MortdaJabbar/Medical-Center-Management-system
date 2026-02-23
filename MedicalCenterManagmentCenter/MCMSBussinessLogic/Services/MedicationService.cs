using MCMSBussinessLogic.Interfaces;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationData _medicationData;

        public MedicationService(IMedicationData medicationData)
        {
            _medicationData = medicationData;
        }

        public Task<bool> CreateAsync(Medication medication)
        {
            return _medicationData.InsertMedicationAsync(medication.DTO);
        }

        public Task<bool> UpdateAsync(Medication medication)
        {
            return _medicationData.UpdateMedicationAsync(medication.DTO);
        }

        public async Task<Medication?> FindByIdAsync(int medicationId)
        {
            var dto = await _medicationData.GetMedicationByIdAsync(medicationId);
            return dto != null ? new Medication(dto) : null;
        }

        public async Task<List<Medication>> GetAllAsync()
        {
            var dtos = await _medicationData.GetAllMedicationsAsync();
            return dtos.Select(dto => new Medication(dto)).ToList();
        }

        public Task<bool> DeleteAsync(int medicationId)
        {
            return _medicationData.DeleteMedicationAsync(medicationId);
        }
    }
}
