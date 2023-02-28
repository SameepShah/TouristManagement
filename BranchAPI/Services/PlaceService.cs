using BranchAPI.Models;
using BranchAPI.Services.Interfaces;
using Microsoft.Azure.Cosmos;

namespace BranchAPI.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly ICosmosDBService _cosmosService;
        private readonly Container _container;
        public PlaceService(ICosmosDBService cosmosService)
        {
            _cosmosService = cosmosService;
            _container = _cosmosService.GetContainer("Places");
        }

        /// <summary>
        /// Get All Places (Move this method to CQRS (Query Part)
        /// Create Method that calls CQRS Query and get response
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Place>> GetAllAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Place>(new QueryDefinition(queryString));
            List<Place> results = new List<Place>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
    }
}
