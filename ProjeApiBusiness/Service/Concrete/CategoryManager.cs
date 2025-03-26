using AutoMapper;
using ProjeApiBusiness.Service.Abstract;
using ProjeApiDal.Context;
using ProjeApiDal.Repository.Concrete;
using ProjeApiModel.DTOs.Data;
using ProjeApiModel.ViewModel;

namespace ProjeApiBusiness.Service.Concrete
{
    public class CategoryManager : RepositoryDTO<Category, CategoryDTO>, ICategoryService
    {
        public CategoryManager(ApiContext apiContext, IMapper mapper)
            : base(apiContext, apiContext.Set<Category>(), apiContext.Set<CategoryDTO>(), mapper)
        {
        }
    }
}
