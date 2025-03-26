using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjeApiBusiness.Service.Abstract;
using ProjeApiDal.Context;
using ProjeApiDal.Repository.Abstract;
using ProjeApiDal.Repository.Concrete;
using ProjeApiModel.DTOs.Data;
using ProjeApiModel.DTOs.IdentityDTO;
using ProjeApiModel.ViewModel;
using ProjeApiModel.ViewModel.Identity;

namespace ProjeApiBusiness.Service.Concrete
{
    public class RoleManager : RepositoryDTO<Role, RoleDTO>, IRoleService
    {
        public RoleManager(ApiContext apiContext, IMapper mapper)
         : base(apiContext, apiContext.Set<Role>(), apiContext.Set<RoleDTO>(), mapper)
        {
        }
    }
}
