using System.Threading.Tasks;

namespace MCMSDAL.Interfaces
{
    public interface IPasswordResetData
    {
        Task CreateResetTokenAsync(RestPasswordTokenDto dto);
        Task<RestPasswordTokenDto?> GetByTokenAsync(string token);
        Task DeleteTokenAsync(string token);
    }
}
