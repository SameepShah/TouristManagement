using AuthenticationWebApi.Models;

namespace AuthenticationWebApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<List<User>> GetAllAsync(string queryString);
    }
}
