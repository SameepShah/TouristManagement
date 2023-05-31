using AuthenticationManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthenticationManager.Models;

namespace AuthenticationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        public AccountController(JwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;   
        }

        /// <summary>
        /// Authentication
        /// </summary>
        /// <param name="authenticateRequest"></param>
        /// <returns></returns>
        [Route("authenticate")]
        [HttpPost]
        public ActionResult<AuthenticationResponse?> Authenticate(AuthenticationRequest authenticateRequest) 
        {
            var authenticationResponse = _jwtTokenHandler.GenerateJwtToken(authenticateRequest);
            if (authenticationResponse == null)
                return Unauthorized();
            return Ok(authenticationResponse);
        }

    }
}
