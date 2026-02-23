using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IMedicationData
    {
        Task<bool> InsertMedicationAsync(MedicationDto medication);
        Task<bool> UpdateMedicationAsync(MedicationDto medication);
        Task<MedicationDto?> GetMedicationByIdAsync(int id);
        Task<List<MedicationDto>> GetAllMedicationsAsync();
        Task<bool> DeleteMedicationAsync(int id);
    }
}
