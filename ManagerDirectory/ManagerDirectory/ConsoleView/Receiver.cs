using System;
using System.IO;
using System.Threading.Tasks;
using ManagerDirectory.Validation;

namespace ManagerDirectory.ConsoleView
{
    internal sealed class Receiver
    {
        private readonly CustomValidation _validation;

        public Receiver(CustomValidation validation)
        {
            _validation = validation;
        }

        internal async Task<(string command, string path)> ReceiveAsync(string defaultPath)
        {
            var bs = 512;
            using var sr = new StreamReader(Console.OpenStandardInput(), bufferSize: bs);
            var valid = true;
            var entries = new string[2];
            var command = string.Empty;
            var path = defaultPath;

            do
            {
                Console.Write($@"{Environment.UserName}#{defaultPath}> ");
                entries = (await sr.ReadLineAsync())!
                    .Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);

                if (entries.Length != 0)
                {
                    command = entries[0];

                    if (entries.Length > 1)
                        path =entries[1];

                    valid = await _validation.CheckForCommandAsync(command);
                }
            } while (!valid);

            return (command, path);
        }
    }
}