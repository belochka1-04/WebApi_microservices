using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Other
{
    public class AuthClientOptions
    {
        public string BaseUrl { get; set; } = null!;
        public string TokenEndpoint { get; set; } = "/api/auth/token";
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string Scope { get; set; } = "user.read";
    }
}
