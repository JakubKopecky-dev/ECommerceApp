using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Interfaces.Services;
using ProductService.Application.Mapping;
using ProductService.Application.Services;
using ProductServiceService = ProductService.Application.Services.ProductService;


namespace ProductService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IProductService, ProductServiceService>();
            
            // Register AutoMapper
            services.AddAutoMapper(cfg => { },typeof(AutomapperConfigurationProfile));

            return services;
        }
    }
}
