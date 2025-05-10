using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Siska.Admin.Application.Services.System;
using Siska.Admin.Application.Services.System.Implementations;
using Siska.Admin.Storage;
using Siska.Admin.Storage.Implementations;
using Siska.Admin.Utility;

namespace Siska.Admin.Application
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            _ = services.AddTransient<IExtractUser, ExtractUser>();
            _ = services.AddSingleton<TokenGenerator>();

            _ = services.AddTransient<IStorage, LocalStorage>();

            _ = services.AddTransient<IUserService, UserService>();
            _ = services.AddTransient<IRoleService, RoleService>();
            _ = services.AddTransient<IAPIEndpointService, APIEndpointService>();

            return services;
        }
    }
}
