using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiModel.Core;
using ProjeApiModel.ViewModel;

namespace ProjeApiModel.DTOs.Data
{
    public class ProductDTO:EntityBase
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}
