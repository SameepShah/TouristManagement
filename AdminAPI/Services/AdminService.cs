using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Microsoft.Azure.Cosmos;

namespace AdminAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICosmosDBService _cosmosService;
        private readonly Container _container;

        public AdminService(ICosmosDBService cosmosService)
        {
            _cosmosService = cosmosService;
            _container = _cosmosService.GetContainer("Branches");
        }

        /// <summary>
        /// Get All Branches 
        /// Create Method that calls CQRS Query and get response
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<List<Branch>> GetAllAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Branch>(new QueryDefinition(queryString));
            List<Branch> results = new List<Branch>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
    }
}
