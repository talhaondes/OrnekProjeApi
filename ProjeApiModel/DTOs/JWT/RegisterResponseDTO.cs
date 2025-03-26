using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjeApiModel.DTOs.JWT
{
    public class RegisterResponseDTO
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public string AdSoyad { get; set; }

    }
}
