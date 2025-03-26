using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeApiModel.DTOs.IdentityDTO
{
    public class UserDTO
    {
        public int Id { get; set; }     
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NameSurName { get; set; }
        public List<string> Roles { get; set; } = new(); 
        public string RoleName { get; set; }
        public string? PasswordHash { get; set; }
    }

}
