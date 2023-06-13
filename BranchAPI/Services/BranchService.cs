using BranchAPI.Messaging;
using BranchAPI.Models;
using BranchAPI.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace BranchAPI.Services
{
    public class BranchService : IBranchService
    {
        private readonly ICosmosDBService _cosmosService;
        private readonly Container _container;
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<BranchService> _logger;
        public BranchService(ICosmosDBService cosmosService, IMessageProducer messageProducer, ILogger<BranchService> logger)
        {
            _cosmosService = cosmosService;
            _messageProducer = messageProducer;
            _container = _cosmosService.GetContainer("Branches");
            _logger = logger;
        }

        /// <summary>
        /// Get All Branches 
        /// Create Method that calls CQRS Query and get response
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Branch>> GetAllAsync(string queryString)
        {
            _logger.LogInformation($"Branch Service: GetAllAsync Called - {DateTime.UtcNow.ToString()}");
            var query = _container.GetItemQueryIterator<Branch>(new QueryDefinition(queryString));
            List<Branch> results = new List<Branch>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            _logger.LogInformation($"Branch Service: GetAllAsync Call Completed - {DateTime.UtcNow.ToString()}");
            return results;
        }

        private Branch DefaultBranchProperties(Branch branch, bool isUpdate)
        {
            try
            {
                branch.id = Guid.NewGuid().ToString();
                branch.CreatedDate = DateTime.Now;
                branch.ModifiedDate = isUpdate ? DateTime.Now : null;
                return branch;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Branch Service Exception: DefaultBranchProperties - {DateTime.UtcNow.ToString()}", ex);
                return branch;
            }
        }


        /// <summary>
        /// Add Branch Details and Send to RabbitMQ Queue
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public async Task<BranchAddResponse> AddBranchAsync(Branch branch)
        {
            _logger.LogInformation($"Branch Service: AddBranchAsync Called - {DateTime.UtcNow.ToString()}");
            BranchAddResponse branchResponse = new BranchAddResponse();
            try
            {
                branch = DefaultBranchProperties(branch, false);
                var req = await _container.CreateItemAsync<Branch>(branch);
                if (req.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    branchResponse.StatusCode = req.StatusCode;
                    branchResponse.BranchId = branch.id ?? string.Empty;
                    var branches = await GetAllAsync("SELECT * FROM c");
                    //Send All Branchs to RabbitMQ Queue
                    _messageProducer.SendMessage("branch", branches);
                }
                _logger.LogInformation($"Branch Service: AddBranchAsync Call Completed - {DateTime.UtcNow.ToString()}");
                return branchResponse;
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Branch Service Exception: AddBranchAsync - {DateTime.UtcNow.ToString()}", ex);
                branchResponse.ErrorMessage = ex.Message;
                branchResponse.StatusCode = ex.StatusCode;
                branchResponse.BranchId = string.Empty;
                return branchResponse;
            }
        }

        /// <summary>
        /// Get Branch By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Branch> GetBranchAsync(string id, string branchCode)
        {
            try
            {
                ItemResponse<Branch> response = await _container.ReadItemAsync<Branch>(id, new PartitionKey(branchCode));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw;
            }
        }


        private Branch UpdateBranchProperties(Branch branch, UpdateBranch entity)
        {
            try
            {
                branch.Places = entity.Places;
                branch.ModifiedDate = DateTime.Now;
                return branch;
            }
            catch (Exception ex)
            {
                return branch;
            }
        }

        /// <summary>
        /// Update Branch Details and Send to RabbitMQ Queue
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<BranchEditResponse> UpdateBranchAsync(UpdateBranch entity)
        {
            _logger.LogInformation($"Branch Service: UpdateBranchAsync Called - {DateTime.UtcNow.ToString()}");
            BranchEditResponse branchEditResponse = new BranchEditResponse();
            try
            {
                var branch = await GetBranchAsync(entity.Id ?? string.Empty,entity.BranchCode ?? string.Empty);
                if (branch != null)
                {
                    branch = UpdateBranchProperties(branch, entity);
                    Branch updatedBranch =  await _container.UpsertItemAsync<Branch>(branch, new PartitionKey(entity.BranchCode));
                    branchEditResponse.BranchId = updatedBranch.id;
                    branchEditResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    var branches = await GetAllAsync("SELECT * FROM c");
                    //Send All Branchs to RabbitMQ Queue
                    _messageProducer.SendMessage("branch", branches);
                }
                _logger.LogInformation($"Branch Service: UpdateBranchAsync Call Completed - {DateTime.UtcNow.ToString()}");
                return branchEditResponse;
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Branch Service Exception: UpdateBranchAsync - {DateTime.UtcNow.ToString()}", ex);
                branchEditResponse.ErrorMessage = ex.Message;
                branchEditResponse.StatusCode = ex.StatusCode;
                branchEditResponse.BranchId = string.Empty;
                return branchEditResponse;
            }
        }


    }
}
