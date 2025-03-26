using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjeApiBusiness.Service.Abstract;
using ProjeApiDal.Context;
using ProjeApiDal.Repository.Concrete;
using ProjeApiModel.DTOs.Data;
using ProjeApiModel.ViewModel;

namespace ProjeApiBusiness.Service.Concrete
{
    public class ProductManager : RepositoryDTO<Product, ProductDTO>, IProductService
    {
        public ProductManager(ApiContext apiContext, IMapper mapper)
            : base(apiContext, apiContext.Set<Product>(), apiContext.Set<ProductDTO>(), mapper)
        {
        }
    }
}
