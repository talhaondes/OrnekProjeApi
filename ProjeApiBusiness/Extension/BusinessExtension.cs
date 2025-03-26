using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjeApiBusiness.Service.Abstract;
using ProjeApiBusiness.Service.Concrete;

namespace ProjeApiBusiness.Extension
{
    public static class BusinessExtension
    {
        public static IServiceCollection LoadBllLayerExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(BusinessExtension).Assembly);

            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<IUserService, UserManager>();
            services.AddScoped<IRoleService, RoleManager>();

            return services;
        }
    }

}
