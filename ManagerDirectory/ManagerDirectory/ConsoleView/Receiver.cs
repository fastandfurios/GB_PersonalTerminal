using System;
using System.Threading.Tasks;
using ManagerDirectory.Validation;

namespace ManagerDirectory.ConsoleView
{
    internal sealed class Receiver
    {
        internal async Task<string> Receive(string defaultPath, CustomValidation validation)
        {
            return await Task.Run(async () =>
            {
                bool valid;
                var entry = string.Empty;

                do
                {
                    Console.Write($"{defaultPath}> ");
                    entry = Console.ReadLine();
                    valid = await validation.CheckForInputAsync(entry);
                } while (!valid);

                return entry;
			});
        }
    }
}
