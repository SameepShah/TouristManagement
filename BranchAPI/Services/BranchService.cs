﻿using BranchAPI.Models;
using BranchAPI.Services.Interfaces;
using Microsoft.Azure.Cosmos;

namespace BranchAPI.Services
{
    public class BranchService : IBranchService
    {
        private readonly ICosmosDBService _cosmosService;
        private readonly Container _container;
        public BranchService(ICosmosDBService cosmosService)
        {
            _cosmosService = cosmosService;
            _container = _cosmosService.GetContainer("Branches");
        }

        /// <summary>
        /// Get All Branches (Move this method to CQRS (Query Part)
        /// Create Method that calls CQRS Query and get response
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Branch>> GetAllAsync(string queryString)
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
