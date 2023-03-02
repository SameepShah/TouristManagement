using AdminAPI.Messaging;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.CompilerServices;

namespace AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IMessageConsumer _consumer;

        public AdminController(IAdminService adminService, IMessageConsumer consumer)
        {
            _adminService = adminService;
            _consumer = consumer;
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
            _consumer.ConsumeMessage("branch");
            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "Test"));
        }

    }
}
