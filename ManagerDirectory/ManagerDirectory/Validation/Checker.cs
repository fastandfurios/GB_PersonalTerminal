using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerDirectory.Actions;

namespace ManagerDirectory.Validation
{
    public class Checker
    {
        private readonly Commands _commands = new();

		public async Task<bool> CheckInputCommand(string nameCommand)
        {
            return await Task.Run(() =>
            {
                foreach (var command in _commands.ArrayCommands)
                {
                    if ((command == nameCommand.Split(" ")[0] && nameCommand != string.Empty) || nameCommand.Contains(':'))
                        return true;
                }

                return false;
			});
        }

		public async Task<string> CheckPath(string path, string defaultPath)
        {
            return await Task.Run(() =>
            {
                if (Directory.Exists(defaultPath + path))
                    return defaultPath + path;

                if ((Directory.Exists(path) || File.Exists(path)) && path.Contains('\\'))
                    return path;

                return defaultPath;
			});
        }
	}
}
