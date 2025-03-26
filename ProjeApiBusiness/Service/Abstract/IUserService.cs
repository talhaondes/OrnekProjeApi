using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiDal.Repository.Abstract;
using ProjeApiModel.DTOs.IdentityDTO;
using ProjeApiModel.DTOs.Response;
using ProjeApiModel.ViewModel.Identity;

namespace ProjeApiBusiness.Service.Abstract
{
    public interface IUserService : IRepositoryDTO<UserDTO>
    {
        Task<ResponseDTO<User>> GetByUserName(string username);

    }
}
