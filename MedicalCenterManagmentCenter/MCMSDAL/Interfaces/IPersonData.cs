using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IPersonData
    {
        Task<List<PersonDTO>> GetAllPeopleAsync(int pageNumber = 1, int pageSize = 10);
        Task<PersonDTO?> GetPersonByIdAsync(Guid personId);
        Task<Guid> AddPersonAsync(PersonDTO person);
        Task<bool> UpdatePersonAsync(PersonDTO person);
        Task<bool> DeletePersonAsync(Guid personId);
        Task<bool> IsPersonExistsByIdAsync(Guid personId);
        Task<bool> IsPersonExistsByNameAsync(string firstName, string secondName, string? thirdName = null);
        Task<PersonDTO?> GetPersonByNameAsync(string firstName, string secondName, string? thirdName = null);
        Task<PersonProfileDto?> GetProfileByIdAsync(Guid personId);
    }
}
