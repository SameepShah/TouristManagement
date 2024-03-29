﻿using AdminAPI.Messaging;
using AdminAPI.Models;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IMessageConsumer _consumer;
        private readonly IDistributedCache _cache;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, IMessageConsumer consumer, IDistributedCache cache, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _consumer = consumer;
            _cache = cache;
            _logger = logger;
        }

        //Controller Endpoints here
        /// <summary>
        /// Consume Message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("consume")]
        public async Task<IActionResult> ConsumeMessage()
        {
            //Get from RedisCache else from Database
            string? serializedData = null;
            var dataAsByteArray = await _cache.GetAsync("branches");
            List<Branch> branches = new List<Branch>();
            if ((dataAsByteArray?.Count() ?? 0) > 0)
            {
                serializedData = Encoding.UTF8.GetString(dataAsByteArray!);
                branches = JsonSerializer.Deserialize<List<Branch>>(serializedData);
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, branches));
            }
            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, Messages.NO_BRANCHES_REDIS));
        }

        /// <summary>
        /// Search Branches with Places
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> SearchPlaces(SearchBranch searchCriteria)
        {
            _logger.LogInformation($"Admin Controller: SearchPlaces Called - {DateTime.UtcNow.ToString()}");
            SearchBranchResponse searchBranchResponse = new SearchBranchResponse();
            //Get from RedisCache else from Database
            string? serializedData = null;
            byte[]? dataAsByteArray = null;
            try
            {
                dataAsByteArray = await _cache.GetAsync("branches");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Admin Controller Exception: SearchPlaces Redis Cache Exception - {DateTime.UtcNow.ToString()}", ex);
            }
            List<Branch> branches = new List<Branch>();
            if ((dataAsByteArray?.Count() ?? 0) > 0)
            {
                serializedData = Encoding.UTF8.GetString(dataAsByteArray!);
                branches = JsonSerializer.Deserialize<List<Branch>>(serializedData);
            }
            else
            {
                //Get All Branches from Database
                branches = await _adminService.GetAllAsync("SELECT * FROM c");
            }
            if (branches.Any())
            {
                //Logic for Filtering Data based on Search Criteria
                try
                {
                    var branchesResult = branches.Where(x => (!String.IsNullOrEmpty(searchCriteria.id) ? x.id!.ToLower() == searchCriteria.id.ToLower() : true) &&
                                                             (!String.IsNullOrEmpty(searchCriteria.BranchCode) ? x.BranchCode!.ToLower() == searchCriteria.BranchCode.ToLower() : true) &&
                                                             (!String.IsNullOrEmpty(searchCriteria.BranchName) ? x.BranchName!.ToLower() == searchCriteria.BranchName.ToLower() : true) &&
                                                             (!String.IsNullOrEmpty(searchCriteria.Place) ? x.Places!.Any(p => p.PlaceName.ToLower() == searchCriteria.Place.ToLower()) : true)).ToList();
                    searchBranchResponse.TotalRecords = branchesResult.Count;
                    if (branchesResult.Count > 0)
                    {
                        branchesResult = branchesResult.AsQueryable().OrderBy(searchCriteria.PaginationSorting.SortColumn!, searchCriteria.PaginationSorting.SortOrder)
                                                                     .Skip((searchCriteria.PaginationSorting.PageIndex - 1) * searchCriteria.PaginationSorting.PageSize)
                                                                     .Take(searchCriteria.PaginationSorting.PageSize).ToList();

                        //Sort Places with TariffAmount Decending
                        branchesResult.ForEach(x => x.Places = x.Places?.OrderByDescending(y => y.TariffAmount).ToList());

                        searchBranchResponse.Branches = branchesResult;
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, searchBranchResponse));
                    }
                    else
                    {
                        searchBranchResponse.TotalRecords = 0;
                        searchBranchResponse.Branches = branchesResult;
                        searchBranchResponse.Message = Messages.NO_PLACES_FOUND_SEARCH;
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.NotFound, searchBranchResponse));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Admin Controller Exception: SearchPlaces Exception - {DateTime.UtcNow.ToString()}", ex);
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.InternalServerError, ex.Message));
                }
            }
            else
            {
                searchBranchResponse.TotalRecords = 0;
                searchBranchResponse.Branches = branches;
                searchBranchResponse.Message = Messages.NO_PLACES_FOUND;
                return await Task.FromResult(StatusCode((int)HttpStatusCode.NotFound, searchBranchResponse));
            }
        }

    }
}
