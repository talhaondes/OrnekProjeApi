using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjeApiModel.ViewModel;

namespace ProjeApiDal.OREM.CategoryConfigs
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
          builder.Property(x => x.CategoryDescription).IsRequired().HasMaxLength(50);
          builder.Property(x => x.CategoryName).IsRequired().HasMaxLength(25);
        }
    }
}
