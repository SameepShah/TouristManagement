using Microsoft.Azure.Cosmos;

namespace AdminAPI.Services.Interfaces
{
    public interface ICosmosDBService
    {
        Container GetContainer(string containerName);
    }
}
