using MCMSDAL;
using System.ComponentModel.DataAnnotations;

namespace MCMSAPI.dtos.PatientsDto
{
    public class AddUpdatePatientDto
    {

        [Required(ErrorMessage = "Weight is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be a positive number.")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Height must be a positive number.")]
        public decimal Height { get; set; }
        public AddUpdatePersonDto Person { get; set; }
    }
}
