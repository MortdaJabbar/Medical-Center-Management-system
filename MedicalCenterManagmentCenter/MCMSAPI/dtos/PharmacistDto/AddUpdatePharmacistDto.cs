using MCMSDAL;

namespace MCMSAPI.dtos.PharmacistDto
{
    public class AddUpdatePharmacistDto
    {
        public AddUpdatePersonDto Person { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly HireDate { get; set; }
        public int? ExpereinceYears { get; set; }


    }



}
