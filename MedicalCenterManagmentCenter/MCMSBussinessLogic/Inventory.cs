using MCMSDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSBussinessLogic
{
   
        public class Inventory
        {
            public InventoryDto DTO => new InventoryDto
            {
                InventoryID = InventoryID,
                MedicationID = MedicationID,
                Quantity = Quantity,
                UnitOfMeasure = UnitOfMeasure,
                ExpiryDate = ExpiryDate,
                Supplier = Supplier,
                RecivedDate = RecivedDate,

            };

            public int InventoryID { get; set; }
            public int MedicationID { get; set; }
            public int Quantity { get; set; }
            public string? UnitOfMeasure { get; set; }
            public DateOnly ExpiryDate { get; set; }
            public string? Supplier { get; set; }
            public DateOnly RecivedDate { get; set; }

            public Inventory() { }

            public Inventory(InventoryDto dto)
            {
                InventoryID = dto.InventoryID;
                MedicationID = dto.MedicationID;
                Quantity = dto.Quantity;
                UnitOfMeasure = dto.UnitOfMeasure;
                ExpiryDate = dto.ExpiryDate;
                Supplier = dto.Supplier;
                RecivedDate = dto.RecivedDate;
            }

            public async Task<bool> AddNewInventoryAsync()
            {
                if (Quantity < 0)
                    throw new InvalidOperationException("Quantity must be non-negative.");

                var newId = await InventoryData.CreateInventoryAsync(DTO);
                InventoryID = newId;
                return newId > 0;
            }

            public async Task<bool> UpdateInventoryAsync()
            {
                if (Quantity < 0)
                    throw new InvalidOperationException("Quantity must be non-negative.");

                return await InventoryData.UpdateInventoryAsync(DTO);
            }

            public static async Task<Inventory?> FindInventoryByIdAsync(int id)
            {
                var dto = await InventoryData.GetInventoryByIdAsync(id);
                return dto != null ? new Inventory(dto) : null;
            }

            public static async Task<List<Inventory>> GetAllInventoryAsync()
            {
                var dtos = await InventoryData.GetAllInventoryAsync();
                return dtos.Select(dto => new Inventory(dto)).ToList();
            }

            public static async Task<bool> DeleteInventoryByIdAsync(int id)
            {
                return await InventoryData.DeleteInventoryAsync(id);
            }

        public  async static Task<List<InventoryDisplayDto>> GetInventoryDetailsAsync()
        { 
             return await InventoryData.GetAllInventoryDetailsAsync();
        }

    }

}

