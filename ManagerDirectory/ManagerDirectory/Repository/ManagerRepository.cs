using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ManagerDirectory.Models;

namespace ManagerDirectory.Repository
{
    internal sealed class ManagerRepository
    {
        internal async Task<CurrentPath> GetSavedPath(string fileName, CurrentPath currentPath, string defaultPath)
		{
            try
            {
                await using var stream = new FileStream(fileName, FileMode.Open);
                return await JsonSerializer.DeserializeAsync<CurrentPath>(stream);
            }
            catch
            {
                currentPath.Path = defaultPath;
                return currentPath;
            }
        }

        internal async Task SaveCurrentPath(CurrentPath currentPath, string fileName)
        {
            await using var fileStream = File.Open(fileName, FileMode.OpenOrCreate);
            await JsonSerializer.SerializeAsync(fileStream, currentPath, typeof(CurrentPath));
        }

        internal async Task<Help> GetHelp()
        {
            var fileName = "HelpContent.json";
            await using var stream = new FileStream(fileName, FileMode.Open);
            return await JsonSerializer.DeserializeAsync<Help>(stream);
        }
	}
}
