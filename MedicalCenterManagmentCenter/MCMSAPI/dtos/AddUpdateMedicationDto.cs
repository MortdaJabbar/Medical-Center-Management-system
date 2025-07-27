using System.ComponentModel.DataAnnotations;

namespace MCMSAPI.dtos
{
    public class AddUpdateMedicationDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Strength { get; set; } = string.Empty;

        [Required]
        public string DosageForm { get; set; } = string.Empty;
    }

}
