#region usings
using System;
using System.Threading.Tasks;
using ManagerDirectory.ConsoleView;
using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Services;
using ManagerDirectory.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#endregion

namespace ManagerDirectory
{
    class Program
	{
		static async Task Main(string[] args)
        {
	        Console.Title = "PersonalTerminal";

			using IHost host = CreateHostBuilder(args).Build();
            
            if (host.Services.GetService(typeof(ManagerService)) is ManagerService manager)
            {
                await manager.RunAsync();
                await manager.SwitchCommandAsync();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<ManagerService>();
            services.AddTransient<Receiver>();
            services.AddTransient<Displaying>();
            services.AddTransient<SerializeDeserializeService>();
            services.AddTransient<CustomValidation>();
            services.AddTransient<InformingService>();
            services.AddTransient<CurrentPath>();
            services.AddTransient<StartService>();
        }
    }
}
