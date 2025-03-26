using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeApiModel.DTOs.JWT
{
    public class LoginResponseDTO
    {
        public TokenDTO tokenDTO { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new();


    }
}
