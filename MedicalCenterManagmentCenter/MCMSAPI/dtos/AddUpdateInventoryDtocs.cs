using System.ComponentModel.DataAnnotations;

namespace MCMSAPI.dtos
{
    public class AddUpdateInventoryDto
    {
        [Required]
        public int MedicationID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        [Required]
        public DateOnly ExpiryDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Supplier { get; set; } = string.Empty;

        [Required]
        public DateOnly RecivedDate { get; set; }
    }

}
