using AdminAPI.Messaging;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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

        public AdminController(IAdminService adminService, IMessageConsumer consumer, IDistributedCache cache)
        {
            _adminService = adminService;
            _consumer = consumer;
            _cache = cache;
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
            if ((dataAsByteArray?.Count() ?? 0) > 0)
            {
                serializedData = Encoding.UTF8.GetString(dataAsByteArray);
                var branches = JsonSerializer.Deserialize<List<Branch>>(serializedData);
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, branches));
            }
            else {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "No Data in Cache so take it from database."));
            }
        }

    }
}
