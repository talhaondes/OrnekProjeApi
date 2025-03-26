using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiDal.Repository.Abstract;
using ProjeApiModel.DTOs.Data;

namespace ProjeApiBusiness.Service.Abstract
{
    public interface IProductService: IRepositoryDTO<ProductDTO>
    {
    }
}
