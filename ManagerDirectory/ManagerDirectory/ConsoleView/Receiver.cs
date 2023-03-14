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

        internal async Task<(string command, Uri path)> ReceiveAsync(Uri defaultPath)
        {
            var bs = 512;
            using var sr = new StreamReader(Console.OpenStandardInput(), bufferSize: bs);
            var valid = true;
            var entries = new string[2];
            var command = string.Empty;
            var path = new Uri(defaultPath.OriginalString);

            do
            {
                Console.Write($@"{Environment.UserName}#{defaultPath.OriginalString}> ");
                entries = (await sr.ReadLineAsync())!
                    .Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);

                if (entries.Length != 0)
                {
                    command = entries[0];

                    if (entries.Length > 1)
                        Uri.TryCreate(entries[1], UriKind.Relative, out path);

                    valid = await _validation.CheckForCommandAsync(command);
                }
            } while (!valid);

            return (command, path);
        }
    }
}