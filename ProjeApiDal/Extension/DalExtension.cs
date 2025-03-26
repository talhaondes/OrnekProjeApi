using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjeApiDal.Context;
using ProjeApiDal.Repository.Abstract;
using ProjeApiDal.Repository.Concrete;
using ProjeApiModel.DTOs.Data;
using ProjeApiModel.DTOs.IdentityDTO;
using ProjeApiModel.ViewModel;
using ProjeApiModel.ViewModel.Identity;

namespace ProjeApiDal.Extension
{
    public static class DalExtension
    {
        public static IServiceCollection LoadDalLayerExtension(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection connection string is missing");

            services.AddDbContext<ApiContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(15), errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                }));

            // Entity ve DTO çiftlerini bir arada tutmak için
            services.AddScoped<IRepositoryDTO<CategoryDTO>, RepositoryDTO<Category, CategoryDTO>>();
            services.AddScoped<IRepositoryDTO<ProductDTO>, RepositoryDTO<Product, ProductDTO>>();
            services.AddScoped<IRepositoryDTO<UserDTO>, RepositoryDTO<User, UserDTO>>();
            services.AddScoped<IRepositoryDTO<RoleDTO>, RepositoryDTO<Role, RoleDTO>>();

            return services;
        }
    }
}
