using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiDal.Repository.Abstract;
using ProjeApiModel.DTOs.IdentityDTO;

namespace ProjeApiBusiness.Service.Abstract
{
    public interface IRoleService: IRepositoryDTO<RoleDTO>
    {
    }
}
