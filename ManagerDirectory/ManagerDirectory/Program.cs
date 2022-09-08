using System;
using System.Threading.Tasks;
using ManagerDirectory.Services;

namespace ManagerDirectory
{
	class Program
	{
		static async Task Main(string[] args)
        {
            Console.Title = "PersonalTerminal";

			var service = new ManagerService();
            await service.StartAsync();
            await service.RunAsync();
        }
    }
}
