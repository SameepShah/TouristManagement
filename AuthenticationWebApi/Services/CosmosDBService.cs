using AuthenticationWebApi.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationWebApi.Services
{
    public class CosmosDBService : ICosmosDBService
    {
        private readonly CosmosClient _client;
        private readonly string _database;
        //private Container _container { get; set; }
        public CosmosDBService(string url, string primaryKey, string databaseName)
        {
            _database = databaseName;
            _client = new CosmosClient(url, primaryKey);
        }

        /// <summary>
        /// Gets the Container for the Service
        /// </summary>
        /// <param name="containerName"></param>
        public Container GetContainer(string containerName)
        {
            return _client.GetContainer(_database, containerName);
        }

    }
}
