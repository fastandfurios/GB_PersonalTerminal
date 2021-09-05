using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerDirectory.Actions;
using ManagerDirectory.IO;
using ManagerDirectory.Models;
using ManagerDirectory.Repository;
using ManagerDirectory.Validation;

namespace ManagerDirectory
{
    public class Manager : IManager
    {
	    private string _entry;
	    private string _defaultPath = "C:\\";
	    private string _fileName = "CurrentPath.json";
	    private string _fileLogErrors = "LogErrors.txt";

        private readonly Output _output = new();
        private readonly ManagerRepository _managerRepository = new();
        private CurrentPath _currentPath = new();
        private readonly Informer _informer = new();
        private readonly Checker _checker = new();

		public async Task Start()
		{
			await Task.Run( async () =>
            {
                _currentPath = await _managerRepository.GetSavePath(_fileName, _currentPath, _defaultPath);

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (_currentPath.Path.Length > drive.Name.Length)
                    {
                        if (drive.Name == _currentPath.Path.Substring(0, 3))
                            return;
                    }
                    else
                    {
                        if (drive.Name == _currentPath.Path.Substring(0, _currentPath.Path.Length))
                            return;
                    }
                }

                _currentPath.Path = _defaultPath;
            });
        } 

	    public async Task Run()
	    {
			if (File.Exists(_fileName) && !string.IsNullOrEmpty(_currentPath.Path))
				_defaultPath = _currentPath.Path;

            var input = new InputData();
			_entry = await input.Input(_defaultPath, _checker);

			await ToDistribute();
	    }

	    private async Task ToDistribute()
	    {
		    try
		    {
			    var command = await Task.Run(() => _entry.Split(" ")[0]);

				if(command.Contains(':'))
					_defaultPath = command + "\\";
				
			    var path = string.Empty;
			    var newPath = string.Empty;

			    switch (command)
			    {
					case "disk":
						await CallOutput();
						break;
					case "ls":
						path = _entry.Length == command.Length ? _defaultPath : _entry.Remove(0, command.Length + 1);
						path = await _checker.CheckPath(path, _defaultPath);
						await CallOutput(path, 10);
						break;
					case "lsAll":
						path = _entry.Length == command.Length ? _defaultPath : _entry.Remove(0, command.Length + 1);
						path = await _checker.CheckPath(path, _defaultPath);
                        await CallOutput(path, Directory.GetDirectories(path).Length + Directory.GetFiles(path).Length);
						break;
					case "cp":
						path = await Transform(_entry.Remove(0, command.Length + 1));
                        path = path.TrimEnd();
						newPath = _entry.Remove(0, command.Length + path.Length + 2) + "\\";
						await CallCopying(path, newPath);
						break;
					case "clear":
						Console.Clear();
						break;
					case "cd":
						path = _entry.Remove(0, command.Length + 1) + "\\";
						_defaultPath = await _checker.CheckPath(path, _defaultPath);
						break;
					case "cd..":
						path = _defaultPath.Remove(_defaultPath.Length - 1, 1);
						_defaultPath = Directory.GetParent(path)?.FullName;
						break;
					case "cd\\":
						_defaultPath = Directory.GetDirectoryRoot(_defaultPath);
						break;
					case "info":
						await CallInformer(command);
						_output.OutputInfoFilesAndDirectory(_informer);
						break;
					case "rm":
                        await CallDeletion(command);
						break;
				}

				if (command != "exit")
				{
					_currentPath.Path = string.Empty;
					await Run();
				}
				else
				{
					_currentPath.Path = _defaultPath;
					await _managerRepository.SaveCurrentPath(_currentPath, _fileName);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				File.AppendAllText(_fileLogErrors, $"{DateTime.Now.ToString("G")} {e.Message} {e.TargetSite}" );
				File.AppendAllText(_fileLogErrors, Environment.NewLine);
				await Run();
			}
		}

		private async Task CallOutput()
			=> await _output.GetDrives();

		private async Task CallOutput(string path, int maxObjects)
			=> await _output.OutputTree(path, maxObjects);

        private async Task CallCopying(string name, string newPath)
        {
            var copying = new Copying();
			await copying.Copy(_defaultPath, name, newPath);
		}
		
		private async Task CallDeletion(string command)
		{
			var entry = await _checker.CheckPath(_entry.Remove(0, command.Length + 1), _defaultPath);

            var deletion = new Deletion();

			if (Path.GetExtension(entry) != string.Empty)
				deletion.FullPathFile = entry;
			else
				deletion.FullPathDirectory = entry;
		}

		private async Task CallInformer(string command)
		{
			var entry = string.Empty;

			if (_entry.Length == command.Length)
				entry = _defaultPath;
			else
				entry = await _checker.CheckPath(_entry.Remove(0, command.Length + 1), _defaultPath);


			if (Path.GetExtension(entry) != string.Empty)
			{
				_informer.FullPathFile = entry;
				_informer.FullPathDirectory = string.Empty;
			}
			else
			{
				_informer.FullPathDirectory = entry;
				_informer.FullPathFile = string.Empty;
			}
		}

		private async Task<string> Transform(string str)
		{
			return await Task.Run(async () =>
            {
                for (int i = str.Length - 1; i > 0; i--)
                {
                    if (str[i] != ':')
                        str = str.Remove(i, 1);
                    else
                    {
                        await Task.Run(() =>
                        {
                            for (int j = str.Length - 1; j > 0; j--)
                            {
                                if (str[j] != ' ')
                                    str = str.Remove(j, 1);
                                else
                                    break;
                            }
                        });

                        break;
					}
                }

                return str;
			});
        }
    }
}
