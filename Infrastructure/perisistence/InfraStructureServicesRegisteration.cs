using DomainLayer.Contracts___Repo_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using perisistence.Data;
using perisistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perisistence
{
    public static class InfraStructureServicesRegisteration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services ,IConfiguration configuration)
        {
            services.AddDbContext<ToDoDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<IDataSeeding, DataSeeding>();

            services.AddScoped<IToDoRepository, ToDoRepository>();

            return services;
        }
    }
}
