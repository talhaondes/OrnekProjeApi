using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeApiModel.DTOs.JWT
{
    public class TokenDTO
    {
        public string AccessToken { get; set; }= string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
