using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiModel.Core;

namespace ProjeApiModel.DTOs.IdentityDTO
{
    public class RoleDTO : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

