using MCMSBussinessLogic.Interfaces;
using MCMSDAL;
using MCMSDAL.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryData _inventoryData;

        public InventoryService(IInventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        public async Task<int?> CreateAsync(Inventory inventory)
        {
            if (inventory.Quantity < 0)
                throw new InvalidOperationException("Quantity must be non-negative.");

            var newId = await _inventoryData.CreateInventoryAsync(inventory.DTO);
            if (newId <= 0)
                return null;
            inventory.InventoryID = newId;
            return newId;
        }

        public Task<bool> UpdateAsync(Inventory inventory)
        {
            if (inventory.Quantity < 0)
                throw new InvalidOperationException("Quantity must be non-negative.");

            return _inventoryData.UpdateInventoryAsync(inventory.DTO);
        }

        public async Task<Inventory?> FindByIdAsync(int inventoryId)
        {
            var dto = await _inventoryData.GetInventoryByIdAsync(inventoryId);
            return dto != null ? new Inventory(dto) : null;
        }

        public async Task<List<Inventory>> GetAllAsync()
        {
            var dtos = await _inventoryData.GetAllInventoryAsync();
            return dtos.Select(dto => new Inventory(dto)).ToList();
        }

        public Task<List<InventoryDisplayDto>> GetAllDetailsAsync()
        {
            return _inventoryData.GetAllInventoryDetailsAsync();
        }

        public Task<bool> DeleteAsync(int inventoryId)
        {
            return _inventoryData.DeleteInventoryAsync(inventoryId);
        }
    }
}
