using MCMSDAL;

namespace MCMSBussinessLogic
{
    public class Person : IPerson
    {
        private static readonly PersonData _personData = new();

        public PersonDTO PDTO
        {
            get { return new PersonDTO(PersonId, FirstName, SecondName, ThirdName, DateOfBirth, Gender, Phone, Email, ImageLocation); }
        }

        public Guid PersonId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? ThirdName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string GenderText { get { return (Gender) ? "Male" : "Female";  } }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? ImageLocation { get; set; }

        public Person()
        {
        

            PersonId = Guid.Empty;
            FirstName  = "" ;
            SecondName = "" ;
            ThirdName  = "" ;
           DateOfBirth =  DateOnly.MaxValue ;
            Gender     =  false;
            Phone      =  "";
            Email      =  "" ;
            ImageLocation = "";
        }
        public Person(PersonDTO PDTO)
        {
            if (PDTO == null)
                throw new ArgumentNullException(nameof(PDTO));

            PersonId = PDTO.PersonId;
            FirstName = PDTO.FirstName;
            SecondName = PDTO.SecondName;
            ThirdName = PDTO.ThirdName;
            DateOfBirth = PDTO.DateOfBirth;
            Gender = PDTO.Gender;
            Phone = PDTO.Phone;
            Email = PDTO.Email;
            ImageLocation = PDTO.ImageLocation.Replace("\\","/");
        }
        public async Task<bool> AddNewPersonAsync()
        {
            if (await IsPersonExistsByNameAsync(FirstName, SecondName, ThirdName))
            {
                return false;
            }

            Guid result = await _personData.AddPersonAsync(PDTO);
            this.PersonId = result;
            return result != Guid.Empty;
        }
        public static async Task<Person?> FindPersonByIdAsync(Guid personId)
        {
            var personDTO = await _personData.GetPersonByIdAsync(personId);
            return (personDTO != null) ? new Person(personDTO) : null;
        }
        public async Task<bool> UpdatePersonAsync()
        {
            if (!await _personData.IsPersonExistsByIdAsync(this.PersonId)) return false;

            return await _personData.UpdatePersonAsync(PDTO);
        }
        public static async Task<bool> DeletePersonByIdAsync(Guid PersonID)
        {
            return await _personData.DeletePersonAsync(PersonID);
        }
        public static async Task<bool> IsPersonExistsByIdAsync(Guid personId)
        {
            return await _personData.IsPersonExistsByIdAsync(personId);
        }
        public static async Task<bool> IsPersonExistsByNameAsync(string FirstName, string SecondName, string? ThirdName=null)
        {
            return await _personData.IsPersonExistsByNameAsync(FirstName, SecondName, ThirdName);
        }
        public static async Task<Person?> FindPersonByNameAsync(string FirstName, string SecondName, string? ThirdName = null)
        {
            PersonDTO? persondto = await _personData.GetPersonByNameAsync(FirstName, SecondName, ThirdName);
            
            
            return persondto!=null ? new Person(persondto) : null ;
        }
        public static async Task<List<Person>> GetAllPeopleAsync(int PageNumber=1 , int PageSize = 10)
        {
            // Fetch people data from the database
            var personDTOs = await _personData.GetAllPeopleAsync(PageNumber,PageSize);

            // Map the DTOs to domain models
            var people = personDTOs.Select(dto => new Person(dto)).ToList();

            return people;
        }

        public static async Task<PersonProfileDto?> GetProfileAsync(Guid personId)
        {
            return await _personData.GetProfileByIdAsync(personId);
        }


    }

}
