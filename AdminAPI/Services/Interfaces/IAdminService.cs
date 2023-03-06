using AdminAPI.Models;

namespace AdminAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<Branch>> GetAllAsync(string queryString);
    }
}
