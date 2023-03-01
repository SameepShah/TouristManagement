using BranchAPI.Models;

namespace BranchAPI.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<Branch>> GetAllAsync(string queryString);
        Task<BranchAddResponse> AddBranchAsync(Branch branch);
        Task<Branch> GetBranchAsync(string id, string branchCode);
        Task<BranchEditResponse> UpdateBranchAsync(UpdateBranch entity);
    }
}
