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
    public class ManagerRepository
    {
	    private string _currentPath;

		public async Task<CurrentPath> GetSavePath(string fileName, CurrentPath currentPath, string defaultPath)
		{
			try
            {
                await using var stream = new FileStream(fileName, FileMode.Open);
                return await JsonSerializer.DeserializeAsync<CurrentPath>(stream);
			}
			catch { }

			currentPath.Path = defaultPath;
			return currentPath;
		}

        public async Task SaveCurrentPath(CurrentPath currentPath, string fileName)
        {
            await JsonSerializer.SerializeAsync(Stream.Null, currentPath, typeof(CurrentPath));
            await File.WriteAllTextAsync(fileName, _currentPath);
        }
	}
}
