using System;
using System.IO;
using System.Threading.Tasks;

namespace ManagerDirectory.Validation
{
    public sealed class CustomValidation
    {
        private readonly Commands.Commands _commands = new();

		internal async Task<bool> CheckForInputAsync(string cmd)
        {
            return await Task.Run(() =>
            {
                foreach (var command in _commands.ArrayCommands)
                {
                    if ((command.Equals(cmd) && !string.IsNullOrEmpty(cmd)) || cmd.Contains(':'))
                        return true;
                }

                return false;
			});
        }

		internal async Task<Uri> CheckEnteredPathAsync(Uri path, Uri defaultPath)
        {
            return await Task.Run(() =>
            {
                if (Directory.Exists(Path.Combine(defaultPath.OriginalString, path.OriginalString)))
                    return new Uri(Path.Combine(defaultPath.OriginalString, path.OriginalString));

                if ((Directory.Exists(path.OriginalString) || File.Exists(path.OriginalString)) && path.OriginalString.Contains('\\'))
                    return path;

                return defaultPath;
			});
        }
	}
}
