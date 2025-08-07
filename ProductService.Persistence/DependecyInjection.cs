using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Persistence.Repositories;

namespace ProductService.Persistence
{
    public static class DependecyInjection
    {

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(connectionString));



            // Register repository services for DI
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();



            return services;
        }







    }
}
