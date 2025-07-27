using System.ComponentModel.DataAnnotations;

namespace MCMSAPI.dtos
{
    public class AddUpdateTestDto
    {
        [Required]
        public Guid PatientID { get; set; }

        [Required]
        public Guid DoctorID { get; set; }

        [Required]
        public int TestTypeID { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; }

        [Required]
        public int Status { get; set; }

        public string? Notes { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        public string? TestResult { get; set; }
    }

}
