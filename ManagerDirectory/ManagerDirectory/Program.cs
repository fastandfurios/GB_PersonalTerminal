using System;
using System.Threading.Tasks;

namespace ManagerDirectory
{
	class Program
	{
		static async Task Main(string[] args)
        {
            Console.Title = "ManagerDirectory";

			var managerDirectory = new Manager();
            await managerDirectory.StartAsync();
            await managerDirectory.RunAsync();
        }
    }
}
