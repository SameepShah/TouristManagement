using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "QeThWmZq4t7w!z%C&F)J@NcRfUjXn2r5";
        private const int JWT_TOKEN_VALID_MINS = 20;
        private const int JWT_TOKEN_VALID_MAXS = 20;
        public JwtTokenHandler()
        {
        }

        /// <summary>
        /// Generate JWT Token for Provided User Details
        /// </summary>
        /// <param name="authenticationRequest"></param>
        /// <returns></returns>
        public TokenResponse? GenerateJwtToken(TokenRequest authenticationRequest)
        {
            if (string.IsNullOrEmpty(authenticationRequest.UserName) || string.IsNullOrEmpty(authenticationRequest.Role)) {
                return null;
            }
         
            var tokenExpiryTimestamp = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALID_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.Name, authenticationRequest.UserName),
                new Claim("Role", authenticationRequest.Role)
            });

            var siginingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);
            var securityTokenDescriptor = new SecurityTokenDescriptor() { 
                Subject = claimsIdentity,
                SigningCredentials = siginingCredentials,
                Expires = tokenExpiryTimestamp
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);
            return new TokenResponse()
            {
                UserName = authenticationRequest.UserName,
                Role = authenticationRequest.Role,
                JwtToken = token,
                ExpiresIn = (int)tokenExpiryTimestamp.Subtract(DateTime.Now).TotalSeconds
            };
        }

    }
}
