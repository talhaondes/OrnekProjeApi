using ProjeApiModel.Core;
using ProjeApiModel.ViewModel;

namespace ProjeApiModel.DTOs.Data
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }
}
