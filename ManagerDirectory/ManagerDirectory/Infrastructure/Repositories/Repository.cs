using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ManagerDirectory.Infrastructure.Models;

namespace ManagerDirectory.Infrastructure.Repositories
{
    internal sealed class Repository
    {
        internal async Task<CurrentPath> GetPath(string fileName, CurrentPath currentPath, string defaultPath)
		{
            try
            {
                await using var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                return await JsonSerializer.DeserializeAsync<CurrentPath>(stream);
            }
            catch
            {
                currentPath.Path = defaultPath;
                return currentPath;
            }
        }

        internal async Task CreatePath(CurrentPath currentPath, string fileName)
        {
            await using var fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            await JsonSerializer.SerializeAsync(fileStream, currentPath, typeof(CurrentPath));
        }

        internal async Task<Help> GetHelp()
        {
            var fileName = "HelpContent.json";
            await using var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            return await JsonSerializer.DeserializeAsync<Help>(stream);
        }
	}
}
