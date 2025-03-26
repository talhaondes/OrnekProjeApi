using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjeApiModel.Core;
using ProjeApiModel.ViewModel;
using ProjeApiModel.ViewModel.Identity;

namespace ProjeApiDal.Context
{
    public class ApiContext : IdentityDbContext<User, Role, int>
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiContext).Assembly);

            // EntityBase'ten türeyen tüm entity'ler için soft delete filter'ı ekle
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                         .Where(e => typeof(EntityBase).IsAssignableFrom(e.ClrType) && !e.ClrType.IsAbstract))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "x");
                var property = Expression.Property(parameter, nameof(EntityBase.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(property), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
            // Cascade delete sorununu önlemek için ilişkileri yapılandırıyoruz:
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Diğer ilişki yapılandırmalarını ekleyin, örneğin User ile UserRole ilişkisi
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Tabloların adlandırılması: bu işlemleri foreach döngüsünün dışına taşıyoruz
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<User>().ToTable("Kullanıcılar");
            modelBuilder.Entity<Role>().ToTable("Roller");
            modelBuilder.Entity<UserRole>().ToTable("KullanıcıyaAtanmış");
        }

        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }

    }
}
