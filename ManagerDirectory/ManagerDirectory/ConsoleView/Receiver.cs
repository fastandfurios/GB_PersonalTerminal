using System;
using System.Threading.Tasks;
using ManagerDirectory.Validation;

namespace ManagerDirectory.ConsoleView
{
    internal sealed class Receiver
    {
        internal async Task<(string command, Uri path)> ReceiveAsync(Uri defaultPath, CustomValidation validation)
        {
            return await Task.Run(async () =>
            {
                var valid = true;
                var entries = new string[2];
                var command = string.Empty;
                var path = new Uri(defaultPath.OriginalString);

                do
                {
                    Console.Write($"{defaultPath.OriginalString}> ");
                    entries = Console.ReadLine()!
                        .Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);

                    if (entries.Length != 0)
                    {
                        command = entries[0];

                        if (entries.Length > 1)
                            Uri.TryCreate(entries[1], UriKind.Relative, out path);

                        valid = await validation.CheckForInputAsync(command);
                    }
                } while (!valid);

                return (command, path);
			});
        }
    }
}