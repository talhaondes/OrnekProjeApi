using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjeApiModel.ViewModel;

namespace ProjeApiDal.OREM.ProductConfigs
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ProductImage).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Stock).HasMaxLength(10);
            builder.Property(x => x.ProductPrice).HasPrecision(18, 2);
            builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
