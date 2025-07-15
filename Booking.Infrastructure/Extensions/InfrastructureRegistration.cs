using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Repositories.Manager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Extensions
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(config["ConnectionStrings:Default"]);
            });
            services.AddScoped<IRepositoryManager, RepositoryManager>();

            return services;
        }
    }
}
