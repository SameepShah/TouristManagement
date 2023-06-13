using BranchAPI.Services.Interfaces;
using Microsoft.Azure.Cosmos;

namespace BranchAPI.Services
{
    public class CosmosDBService : ICosmosDBService
    {
        private readonly CosmosClient _client;
        private readonly string _database;
        public CosmosDBService(string url, string primaryKey, string databaseName)
        {
            _database = databaseName;
            _client = new CosmosClient(url, primaryKey);
        }
        
        /// <summary>
        /// Gets the Container for the Service
        /// </summary>
        /// <param name="containerName"></param>
        public Container GetContainer(string containerName) {
            return _client.GetContainer(_database, containerName);
        }

    }
}
