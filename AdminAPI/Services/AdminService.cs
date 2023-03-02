using AdminAPI.Services.Interfaces;
using Microsoft.Azure.Cosmos;

namespace AdminAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICosmosDBService _cosmosService;
        //private readonly Container _container;

        public AdminService(ICosmosDBService cosmosService)
        {
            _cosmosService = cosmosService;
        }
    }
}
