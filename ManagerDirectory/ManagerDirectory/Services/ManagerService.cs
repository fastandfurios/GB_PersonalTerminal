using System;
using System.IO;
using System.Threading.Tasks;
using ManagerDirectory.ConsoleView;
using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Infrastructure.Repositories;
using ManagerDirectory.Validation;

namespace ManagerDirectory.Services
{
    internal sealed class ManagerService
    {
        private string _entry;
        private string _defaultPath = "C:\\";
        private string _fileName = "CurrentPath.json";
        private string _fileLogErrors = "LogErrors.txt";

        private readonly Displaying _displaying = new();
        private readonly Repository _repository = new();
        private CurrentPath _currentPath = new();
        private readonly InformingService _informer;
        private readonly CustomValidation _validation = new();

        public ManagerService()
        {
        }

        public ManagerService(InformingService informer) : this()
        {
            _informer = informer;
        }

        public async Task StartAsync()
        {
            await Task.Run(async () =>
            {
                _currentPath = await _repository.GetPath(_fileName, _currentPath, _defaultPath);

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

        public async Task RunAsync()
        {
            if (File.Exists(_fileName) && !string.IsNullOrEmpty(_currentPath.Path))
                _defaultPath = _currentPath.Path;

            var input = new Receiver();
            _entry = await input.Receive(_defaultPath, _validation);

            await ToDistribute();
        }

        private async Task ToDistribute()
        {
            try
            {
                var command = await Task.Run(() => _entry.Split(" ")[0]);

                if (command.Contains(':'))
                    _defaultPath = command + "\\";

                var path = string.Empty;
                var newPath = string.Empty;

                switch (command)
                {
                    case "disk":
                        await CallOutputAsync();
                        break;
                    case "ls":
                        path = _entry.Length == command.Length ? _defaultPath : _entry.Remove(0, command.Length + 1);
                        path = await _validation.CheckEnteredPathAsync(path, _defaultPath);
                        await CallOutputAsync(path, 10);
                        break;
                    case "lsAll":
                        path = _entry.Length == command.Length ? _defaultPath : _entry.Remove(0, command.Length + 1);
                        path = await _validation.CheckEnteredPathAsync(path, _defaultPath);
                        await CallOutputAsync(path, Directory.GetDirectories(path).Length + Directory.GetFiles(path).Length);
                        break;
                    case "cp":
                        path = await TransformAsync(_entry.Remove(0, command.Length + 1));
                        path = path.TrimEnd();
                        newPath = _entry.Remove(0, command.Length + path.Length + 2) + "\\";
                        await CallCopyingAsync(path, newPath);
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "cd":
                        path = _entry.Remove(0, command.Length + 1) + "\\";
                        _defaultPath = await _validation.CheckEnteredPathAsync(path, _defaultPath);
                        break;
                    case "cd..":
                        path = _defaultPath.Remove(_defaultPath.Length - 1, 1);
                        _defaultPath = Directory.GetParent(path)?.FullName;
                        break;
                    case "cd\\":
                        _defaultPath = Directory.GetDirectoryRoot(_defaultPath);
                        break;
                    case "info":
                        await CallInformerAsync(command);
                        await _displaying.OutputInfoFilesAndDirectoryAsync(_informer);
                        break;
                    case "help":
                        var help = await _repository.GetHelp();
                        Console.WriteLine(help);
                        break;
                    case "rm":
                        await CallDeletionAsync(command);
                        break;
                }

                if (command != "exit")
                {
                    _currentPath.Path = string.Empty;
                    await RunAsync();
                }
                else
                {
                    _currentPath.Path = _defaultPath;
                    await _repository.CreatePath(_currentPath, _fileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await File.AppendAllTextAsync(_fileLogErrors, $"{DateTime.Now:G} {e.Message} {e.TargetSite}");
                await File.AppendAllTextAsync(_fileLogErrors, Environment.NewLine);
                await RunAsync();
            }
        }

        private async Task CallOutputAsync()
            => await _displaying.GetDisksAsync();

        private async Task CallOutputAsync(string path, int maxObjects)
            => await _displaying.OutputTreeAsync(path, maxObjects);

        private async Task CallCopyingAsync(string name, string newPath)
        {
            var copying = new CopyingService();
            await copying.Copy(_defaultPath, name, newPath);
        }

        private async Task CallDeletionAsync(string command)
        {
            var entry = await _validation.CheckEnteredPathAsync(_entry.Remove(0, command.Length + 1), _defaultPath);

            var deletion = new RemovingService();

            if (Path.GetExtension(entry) != string.Empty)
                deletion.FullPathFile = entry;
            else
                deletion.FullPathDirectory = entry;
        }

        private async Task CallInformerAsync(string command)
        {
            var entry = string.Empty;

            if (_entry.Length == command.Length)
                entry = _defaultPath;
            else
                entry = await _validation.CheckEnteredPathAsync(_entry.Remove(0, command.Length + 1), _defaultPath);

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

        private async Task<string> TransformAsync(string str)
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
