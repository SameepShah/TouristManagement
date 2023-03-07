using BranchAPI.Models;
using BranchAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BranchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        private readonly IBranchService _branchService;
        private readonly ILogger<BranchController> _logger;
        public BranchController(IPlaceService placeService, IBranchService branchService, ILogger<BranchController> logger)
        {
            _placeService = placeService;
            _branchService = branchService;
            _logger = logger;
        }

        /// <summary>
        /// Get All Places
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("places")]
        public async Task<IActionResult> GetAllPlaces()
        {
            var places = await _placeService.GetAllAsync("SELECT c.PlaceId, c.PlaceName, c.TariffAmount FROM c");

            if (!places.Any())
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "Empty"));
            }

            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, places.ToList()));
        }

        /// <summary>
        /// Get All Branches
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("branches")]
        public async Task<IActionResult> GetAllBranches()
        {
            _logger.LogInformation($"Branch Controller: GetAllBranches Called - {DateTime.UtcNow.ToString()}");
            var branches = await _branchService.GetAllAsync("SELECT * FROM c");

            if (!branches.Any())
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "Empty"));
            }
            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, branches.ToList()));
        }

        /// <summary>
        /// Add Branch
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [Route("addbranch")]
        [HttpPost]
        public async Task<IActionResult> AddBranch(Branch branch)
        {
            _logger.LogInformation($"Branch Controller: AddBranch Called - {DateTime.UtcNow.ToString()}");
            if (ModelState.IsValid)
            {
                if (branch.Places == null || branch.Places.Count <= 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Place details must not be empty."));

                var tariffNullCount = branch.Places.Count(x => x.TariffAmount == null || x.TariffAmount.ToString() == "");
                if(tariffNullCount > 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Tariff must not be empty."));

                var invalidtariffRangeCount = branch.Places.Count(x => x.TariffAmount is < 50000 or > 100000);
                if(invalidtariffRangeCount > 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Tariff must be in range between 50000 to 100000."));

                BranchAddResponse response = await _branchService.AddBranchAsync(branch);
                if (!string.IsNullOrEmpty(response.BranchId))
                {
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, response.BranchId));
                }
                else {
                    return await Task.FromResult(StatusCode((int)response.StatusCode, response.ErrorMessage));
                }
            }
            return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, branch));
        }


        /// <summary>
        /// Update Branch Tariff Details
        /// </summary>
        /// <param name="updateBranch"></param>
        /// <returns></returns>
        [Route("editbranch")]
        [HttpPost]
        public async Task<IActionResult> EditBranch(UpdateBranch updateBranch)
        {
            if (ModelState.IsValid)
            {
                if (updateBranch.Places == null || updateBranch.Places.Count <= 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Place details must not be empty."));

                var tariffNullCount = updateBranch.Places.Count(x => x.TariffAmount == null || x.TariffAmount.ToString() == "");
                if (tariffNullCount > 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Tariff must not be empty."));

                var invalidtariffRangeCount = updateBranch.Places.Count(x => x.TariffAmount is < 50000 or > 100000);
                if (invalidtariffRangeCount > 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Tariff must be in range between 50000 to 100000."));

                var req = await _branchService.UpdateBranchAsync(updateBranch);
                if (string.IsNullOrEmpty(req.BranchId))
                {
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.NotFound, "Branch not found"));
                }

                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, req.BranchId));
            }
            return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, updateBranch));
        }

    }
}
