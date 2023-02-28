using Microsoft.Azure.Cosmos;

namespace BranchAPI.Services.Interfaces
{
    public interface ICosmosDBService
    {
        Container GetContainer(string containerName);
    }
}
