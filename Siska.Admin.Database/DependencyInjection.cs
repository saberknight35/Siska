using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Siska.Admin.Database.Repositories.System;
using Siska.Admin.Database.Repositories.System.Implementations;

namespace Siska.Admin.Database
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            _ = services.AddDbContext<SiskaDbContext>(options => options.UseNpgsql(connectionString));

            _ = services.AddTransient<IAPIEndpointRepository, APIEndpointRepository>();

            return services;
        }
    }
}
