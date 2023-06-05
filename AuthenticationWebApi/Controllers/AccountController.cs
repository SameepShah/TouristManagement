using AuthenticationManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthenticationManager.Models;
using AuthenticationWebApi.Services.Interfaces;
using AuthenticationWebApi.Models;

namespace AuthenticationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IAuthService _authService;
        public AccountController(JwtTokenHandler jwtTokenHandler, IAuthService authService)
        {
            _jwtTokenHandler = jwtTokenHandler;   
            _authService = authService;
        }

        /// <summary>
        /// Authentication
        /// </summary>
        /// <param name="authenticateRequest"></param>
        /// <returns></returns>
        [Route("authenticate")]
        [HttpPost]
        public ActionResult<TokenResponse?> Authenticate(AuthenticationRequest authenticateRequest) 
        {

            List<User> users = Task.Run(() => _authService.GetAllAsync($"select * from c where c.UserName = {authenticateRequest.UserName} and c.Password = {authenticateRequest.Password}")).Result;
            if (users == null)
                return Unauthorized();

            TokenRequest tokenRequest = new TokenRequest(){
                UserName = users.FirstOrDefault()!.UserName,
                Role = users.FirstOrDefault()!.Role
            };

            var authenticationResponse = _jwtTokenHandler.GenerateJwtToken(tokenRequest);
            if (authenticationResponse == null)
                return Unauthorized();
            return Ok(authenticationResponse);
        }

    }
}
