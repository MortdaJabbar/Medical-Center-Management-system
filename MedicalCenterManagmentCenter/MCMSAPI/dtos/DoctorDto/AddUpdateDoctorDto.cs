using MCMSDAL;

namespace MCMSAPI.dtos.DoctorDto
{
    public class AddUpdateDoctorDto
    {
     
            public AddUpdatePersonDto Person { get; set; }
            public string Specialization { get; set; }
            public bool Available { get; set; }
            public int? ScheduleId { get; set; }
            public int? Experienceyears { get; set; }
      
    }
}
