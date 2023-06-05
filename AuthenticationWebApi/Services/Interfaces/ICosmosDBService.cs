using Microsoft.Azure.Cosmos;

namespace AuthenticationWebApi.Services.Interfaces
{
    public interface ICosmosDBService
    {
        Container GetContainer(string containerName);
    }
}
