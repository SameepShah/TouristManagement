using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Microsoft.Azure.Cosmos;

namespace AdminAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICosmosDBService _cosmosService;
        private readonly Container _container;
        private readonly ILogger<AdminService> _logger;
        public AdminService(ICosmosDBService cosmosService, ILogger<AdminService> logger)
        {
            _cosmosService = cosmosService;
            _container = _cosmosService.GetContainer("Branches");
            _logger = logger;
        }

        /// <summary>
        /// Get All Branches 
        /// Create Method that calls CQRS Query and get response
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<List<Branch>> GetAllAsync(string queryString)
        {
            _logger.LogInformation($"Admin Service: GetAllAsync Called - {DateTime.UtcNow.ToString()}");
            var query = _container.GetItemQueryIterator<Branch>(new QueryDefinition(queryString));
            List<Branch> results = new List<Branch>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            _logger.LogInformation($"Admin Service: GetAllAsync Call Completed - {DateTime.UtcNow.ToString()}");
            return results;
        }
    }
}
