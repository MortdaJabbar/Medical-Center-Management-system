using AutoMapper;
using MCMSAPI.dtos.DoctorDto;
using MCMSAPI.dtos.PatientsDto;
using MCMSAPI.dtos.PharmacistDto;
using MCMSDAL;
using MCMSBussinessLogic;


namespace MCMSAPI.dtos.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1. Mapping between AddPersonDto and Person (Entity)
            CreateMap<AddUpdatePersonDto, Person>()
                .ForMember(dest => dest.PersonId, opt => opt.Ignore()); // Because AddPersonDto has no PersonId
            CreateMap<Person, AddUpdatePersonDto>(); // (optional, for reverse if needed)

            // 2. Mapping between AddPersonDto and PersonDTO
            CreateMap<AddUpdatePersonDto, PersonDTO>()
                .ForMember(dest => dest.PersonId, opt => opt.Ignore());
            CreateMap<PersonDTO, AddUpdatePersonDto>();



            // 3. Mapping between AddUpdatePatientDto and Patient (Entity)
            CreateMap<AddUpdatePatientDto, Patient>()
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.PersonId, opt => opt.Ignore()) // still ignore PersonId because it's inside PatientDTO
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.Person.SecondName))
                .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.Person.ThirdName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person.Gender))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Person.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Person.Email))
                .ForMember(dest => dest.ImageLocation, opt => opt.MapFrom(src => src.Person.ImageLocation));

                CreateMap<AddUpdateDoctorDto, Doctor>()
               .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
               .ForMember(dest => dest.Available, opt => opt.MapFrom(src => src.Available))
               .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
               .ForMember(dest => dest.Experienceyears, opt => opt.MapFrom(src => src.Experienceyears))
               .ForMember(dest => dest.PersonId, opt => opt.Ignore()) // still ignore PersonId because it's inside PatientDTO
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person.FirstName))
               .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.Person.SecondName))
               .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.Person.ThirdName))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person.Gender))
               .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Person.Phone))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Person.Email))
               .ForMember(dest => dest.ImageLocation, opt => opt.MapFrom(src => src.Person.ImageLocation));
                
            CreateMap<AddUpdatePharmacistDto, Pharmacist>()
               .ForMember(dest => dest.HireDate, opt => opt.MapFrom(src => src.HireDate))
               .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
               .ForMember(dest => dest.ExpereinceYears, opt => opt.MapFrom(src => src.ExpereinceYears))
               .ForMember(dest => dest.PersonId, opt => opt.Ignore()) // still ignore PersonId because it's inside PatientDTO
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person.FirstName))
               .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.Person.SecondName))
               .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.Person.ThirdName))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person.Gender))
               .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Person.Phone))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Person.Email))
               .ForMember(dest => dest.ImageLocation, opt => opt.MapFrom(src => src.Person.ImageLocation));


            CreateMap<AddUpdateStaffDto, Staff>()
    .ForMember(dest => dest.HireDate, opt => opt.MapFrom(src => src.HireDate))
    .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))

    // تجاهل PersonId لأنه يتم توليده أو البحث عنه لاحقاً
    .ForMember(dest => dest.PersonId, opt => opt.Ignore())

    // Map خصائص Person من AddUpdatePersonDto
    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person.FirstName))
    .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.Person.SecondName))
    .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.Person.ThirdName))
    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth))
    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person.Gender))
    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Person.Phone))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Person.Email))
    .ForMember(dest => dest.ImageLocation, opt => opt.MapFrom(src => src.Person.ImageLocation));



            CreateMap<Medication, MedicationDto>().ReverseMap();
                CreateMap<AddUpdateMedicationDto, Medication>();



                CreateMap<Appointment, AppointmentDto>().ForMember(dest => dest.AppointmentTime, opt => opt.Ignore()).ReverseMap();
            CreateMap<AddUpdateAppintmentdto, Appointment>().ForMember(dest => dest.AppointmentTime, opt => opt.Ignore());  



            CreateMap<TestType, TestTypeDto>().ReverseMap();
                CreateMap<AddUpdateTestTypeDto, TestType>();
                
                CreateMap<Test, TestDto>().ReverseMap();
                CreateMap<AddUpdateTestDto, Test>().ForMember(dest => dest.TestID, opt => opt.Ignore()).ReverseMap();
                
                CreateMap<Prescription, PrescriptionDto>().ReverseMap();
                CreateMap<AddUpdatePrescriptionDto, Prescription>().ForMember(dest => dest.PrescriptionID, opt => opt.Ignore()).ReverseMap();
                
                CreateMap<InventoryDto, Inventory>().ReverseMap();
                CreateMap<AddUpdateInventoryDto, Inventory>().ForMember(dest => dest.InventoryID, opt => opt.Ignore()).ReverseMap();


        }
    }

}
