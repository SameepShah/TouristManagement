﻿namespace AuthenticationManager.Models
{
    public class TokenResponse
    {
        public string UserName { get; set; }
        public string JwtToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
