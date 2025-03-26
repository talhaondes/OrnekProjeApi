using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiModel.Core;

namespace ProjeApiModel.ViewModel
{
    public class Category: EntityBase
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public List<Product> Products { get; set; }

    }
}
