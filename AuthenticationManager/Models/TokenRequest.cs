using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationManager.Models
{
    public class TokenRequest
    {
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
