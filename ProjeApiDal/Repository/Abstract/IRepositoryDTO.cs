using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiModel.DTOs.IdentityDTO;
using ProjeApiModel.DTOs.Response;

namespace ProjeApiDal.Repository.Abstract
{
    public interface IRepositoryDTO<TentityDTO> where TentityDTO : class, new()
    {
        Task<ResponseDTO<TentityDTO>> Get(int id);
        Task<ResponseDTO<List<TentityDTO>>> GetAll();
        Task<ResponseDTO<List<TentityDTO>>> GetAllIgnoeFilters();
        Task<ResponseDTO<TentityDTO>> Create(TentityDTO entity);
        Task<ResponseDTO<TentityDTO>> Update(TentityDTO entity, int id);
        Task<ResponseDTO<TentityDTO>> Delete(int id);
 

    }
}
