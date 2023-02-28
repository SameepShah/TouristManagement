using BranchAPI.Models;

namespace BranchAPI.Services.Interfaces
{
    public interface IPlaceService
    {
        Task<IEnumerable<Place>> GetAllAsync(string queryString);
    }
}
