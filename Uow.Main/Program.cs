using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Uow.Composition;
using Uow.Domain.Core;
using Uow.Domain.UserManagement;

namespace Uow.Main
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            var services = CompositionRoot.Prepare(configuration);
            var provider = services.BuildServiceProvider();

            Log.StaticLogger = provider.GetRequiredService<ILogger<Log>>();

            
            using var scope = provider.CreateScope();
            
            await DoTheMagic(scope.ServiceProvider);
        }

        private static async Task DoTheMagic(IServiceProvider serviceProvider)
        {
            var updater = serviceProvider.GetRequiredService<IRandomUserUpdater>();

            await updater.MessUpSomeUser();
            
            await updater.NestedTransCommitOnBoth();

            await updater.NestedTransRollbackOuter();

            await updater.NestedTransRollbackInner();
        }
    }
}