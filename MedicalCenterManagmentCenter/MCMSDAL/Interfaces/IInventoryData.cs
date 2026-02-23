using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IInventoryData
    {
        Task<int> CreateInventoryAsync(InventoryDto dto);
        Task<bool> UpdateInventoryAsync(InventoryDto dto);
        Task<InventoryDto?> GetInventoryByIdAsync(int inventoryId);
        Task<List<InventoryDto>> GetAllInventoryAsync();
        Task<bool> DeleteInventoryAsync(int inventoryId);
        Task<List<InventoryDisplayDto>> GetAllInventoryDetailsAsync();
    }
}
