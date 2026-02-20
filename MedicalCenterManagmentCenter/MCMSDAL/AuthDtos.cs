using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMSDAL
{
    public class TokenPairDto
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
    }

    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = "";
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
