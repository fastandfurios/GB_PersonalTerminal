using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Properties;

namespace ManagerDirectory.Services
{
    public class SerializeDeserializeService
    {
        private readonly CurrentPath _currentPath;

        public SerializeDeserializeService(CurrentPath currentPath)
        {
            _currentPath = currentPath;
        }

        internal async Task<CurrentPath> GetPathAsync(string fileName, string defaultPath)
        {
            try
            {
                await using var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                return await JsonSerializer.DeserializeAsync<CurrentPath>(stream);
            }
            catch
            {
                _currentPath.Path = defaultPath;
                return _currentPath;
            }
        }

        internal async Task SavePathAsync(string fileName, string currentPath)
        {
            _currentPath.Path = currentPath;
            await using var fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            await JsonSerializer.SerializeAsync(fileStream, _currentPath, typeof(CurrentPath));
        }

        internal async Task<Help> GetHelpAsync()
        {
            await using var stream = new FileStream(Resources.HelpContent, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            return await JsonSerializer.DeserializeAsync<Help>(stream);
        }
    }
}
