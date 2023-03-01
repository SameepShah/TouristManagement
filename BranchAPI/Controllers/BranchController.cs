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
        public BranchController(IPlaceService placeService, IBranchService branchService)
        {
            _placeService = placeService;
            _branchService = branchService;
        }

        /// <summary>
        /// Get All Places
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("places")]
        public async Task<IActionResult> GetAllPlaces()
        {
            var places = await _placeService.GetAllAsync("SELECT c.id, c.PlaceId, c.PlaceName FROM c");

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
            if (ModelState.IsValid)
            {
                if(branch.Tariffs == null || branch.Tariffs.Count <= 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Tariff details must not be empty."));

                var invalidtariffRangeCount = branch.Tariffs.Count(x => x.TariffAmount is < 50000 or > 100000);
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
                if (updateBranch.Tariffs == null || updateBranch.Tariffs.Count <= 0)
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "Tariff details must not be empty."));

                var invalidtariffRangeCount = updateBranch.Tariffs.Count(x => x.TariffAmount is < 50000 or > 100000);
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
