using AuthenticationWebApi.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace AuthenticationWebApi.Services;
public class AuthService : IAuthService
{
    private ICosmosDBService _cosmosService;
    private readonly Container _container;
    private readonly ILogger<AuthService> _logger;
    public AuthService(ICosmosDBService cosmosDBService, ILogger<AuthService> logger)
    {
        _cosmosService = cosmosDBService;
        _container = _cosmosService.GetContainer("Users");
        _logger = logger;

    }
    /// <summary>
    /// Get All Users from Database
    /// </summary>
    /// <param name="queryString"></param>
    /// <returns></returns>
    public async Task<List<Models.User>> GetAllAsync(string queryString)
    {
        _logger.LogInformation($"Auth Service: GetAllAsync Called - {DateTime.UtcNow.ToString()}");
        var query = _container.GetItemQueryIterator<Models.User>(new QueryDefinition(queryString));
        List<Models.User> results = new List<Models.User>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        _logger.LogInformation($"Auth Service: GetAllAsync Call Completed - {DateTime.UtcNow.ToString()}");
        return results;
    }

   
}
