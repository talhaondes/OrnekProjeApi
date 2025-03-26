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
using ProjeApiModel.DTOs.Response;
using ProjeApiModel.ViewModel;
using ProjeApiModel.ViewModel.Identity;

namespace ProjeApiBusiness.Service.Concrete
{
    public class UserManager : RepositoryDTO<User, UserDTO>, IUserService
    {
        public UserManager(ApiContext apiContext, IMapper mapper)
                : base(apiContext, apiContext.Set<User>(), apiContext.Set<UserDTO>(), mapper)
        {
        }

        public async Task<ResponseDTO<User>> GetByUserName(string username)
        {
            try
            {
                var user = await _table.FirstOrDefaultAsync(x => x.UserName == username);

                if (user == null)
                {
                    return new ResponseDTO<User>
                    {
                        StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                        Messages = "Kullanıcı bulunamadı"
                    };
                }

                return new ResponseDTO<User>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Success,
                    Data = user,
                    Messages = "Kullanıcı bulundu"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO<User>
                {
                    StatusCode = ProjeApiModel.Enums.StatusCode.Error,
                    Messages = $"Hata: {ex.Message}"
                };
            }
        }

    }
}
