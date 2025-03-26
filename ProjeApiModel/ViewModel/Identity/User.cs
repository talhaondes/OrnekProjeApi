using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProjeApiModel.ViewModel.Identity
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        private string NameSurName { get; set; }
          
    }
}
