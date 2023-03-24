using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Infrastructure.Repositories;
using ManagerDirectory.Properties;
using System.IO;
using System.Threading.Tasks;

namespace ManagerDirectory.Services
{
	public class StartService
	{
		private readonly Repository _repository;
		private CurrentPath _currentPath;
		private readonly string _fileName = Resources.CurrentPath;

		public StartService(Repository repository, CurrentPath currentPath)
		{
			_repository = repository;
			_currentPath = currentPath;
		}

		public async Task StartAsync(string defaultPath)
		{
			if (File.Exists(_fileName))
				_currentPath = await _repository.GetPathAsync(_fileName, defaultPath);
			else
			{
				File.Create(_fileName).Close();
				_currentPath.Path = defaultPath;
			}

			foreach (var drive in DriveInfo.GetDrives())
			{
				if (_currentPath.Path.Length > drive.Name.Length)
				{
					if (drive.Name.Equals(_currentPath.Path.Substring(0, 3)))
						return;
				}
				else
				{
					if (drive.Name.Equals(_currentPath.Path.Substring(0, _currentPath.Path.Length)))
						return;
				}
			}

			_currentPath.Path = defaultPath;
		}

		public string GetCurrentPath() => _currentPath.Path;
	}
}
