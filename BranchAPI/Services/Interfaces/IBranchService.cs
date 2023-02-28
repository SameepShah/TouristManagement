using BranchAPI.Models;

namespace BranchAPI.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<Branch>> GetAllAsync(string queryString);
    }
}
