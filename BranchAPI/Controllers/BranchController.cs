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
        /// 
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
    }
}
