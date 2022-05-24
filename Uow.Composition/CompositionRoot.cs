using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Uow.Data.Core;
using Uow.Data.Repositories;
using Uow.Domain.Core;
using Uow.Domain.UserManagement;

namespace Uow.Composition
{
    public static class CompositionRoot
    {
        public static IServiceCollection Prepare(IConfigurationRoot configurationRoot)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IDbSettings>(sp => new SampleDbSettings(configurationRoot));
            services.AddTransient<IDb, SampleDb>();
            services.AddScoped<IDbUnitOfWork, DbUnitOfWork>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<IDbUnitOfWork>()); // daisy chain
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<IRandomUserUpdater, RandomUserUpdater>();
            return services;
        }
    }
}