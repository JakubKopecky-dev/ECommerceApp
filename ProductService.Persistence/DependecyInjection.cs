using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductService.Persistence
{
    public static class DependecyInjection
    {

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(connectionString));



            // DI Repostitories




            return services;
        }







    }
}
