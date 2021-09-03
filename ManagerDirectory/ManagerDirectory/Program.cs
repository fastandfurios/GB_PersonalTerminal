using System;
using System.Threading.Tasks;
using ManagerDirectory.Actions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ManagerDirectory
{
	class Program
	{
		static async Task Main(string[] args)
        {
            //using var host = CreateHostBuilder(args).Build();

			Console.Title = "ManagerDirectory";

			var managerDirectory = new Manager();
            await managerDirectory.Run();
            managerDirectory.Start();

            //await host.RunAsync();
        }

        //private static IHostBuilder CreateHostBuilder(string[] args)
        //    => Host.CreateDefaultBuilder(args)
        //        .ConfigureServices((_, services) => 
        //            services.AddSingleton<IManager, Manager>());
    }
}
