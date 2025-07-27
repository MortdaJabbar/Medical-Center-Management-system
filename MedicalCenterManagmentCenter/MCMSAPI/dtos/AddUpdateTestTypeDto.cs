using System.ComponentModel.DataAnnotations;

namespace MCMSAPI.dtos
{
    public class AddUpdateTestTypeDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative.")]
        public decimal Cost { get; set; }
    }

}
