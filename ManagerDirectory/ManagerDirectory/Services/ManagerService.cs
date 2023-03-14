using System;
using System.IO;
using System.Threading.Tasks;
using ManagerDirectory.ConsoleView;
using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Infrastructure.Repositories;
using ManagerDirectory.Properties;
using ManagerDirectory.Validation;
using cm = ManagerDirectory.Commands.Commands;

namespace ManagerDirectory.Services
{
    internal sealed class ManagerService
    {
        #region fields
        private (string command, Uri path) _entry;
        private Uri _defaultPath = new(Resources.DefaultPath);
        private readonly string _fileName = Resources.CurrentPath;
        private readonly string _fileLogErrors = Resources.FileLogErrors;
        private readonly Displaying _displaying;
        private readonly Receiver _receiver;
        private readonly Repository _repository;
        private CurrentPath _currentPath;
        private readonly InformingService _informer;
        private readonly CustomValidation _validation;
        #endregion

        public ManagerService(Displaying displaying, 
            Receiver receiver, 
            Repository repository,
            CustomValidation validation,
            InformingService informer,
            CurrentPath currentPath)
        {
            (_displaying, _receiver, _repository, _validation, _informer, _currentPath) = (displaying, receiver, repository, validation, informer, currentPath);
        }

        public async Task StartAsync()
        {
            await Task.Run(async () =>
            {
                if(File.Exists(_fileName))
                    _currentPath = await _repository.GetPathAsync(_fileName, _defaultPath);
                else
                {
					File.Create(_fileName).Close();
                    _currentPath.Path = _defaultPath.OriginalString;
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

                _currentPath.Path = _defaultPath.OriginalString;
            });
        }

        public async Task RunAsync()
        {
            if (File.Exists(_fileName) && !string.IsNullOrEmpty(_currentPath.Path))
                _defaultPath = new Uri(_currentPath.Path);

            _entry = await _receiver.ReceiveAsync(_defaultPath);

            await SwitchCommandAsync();
        }

        private async Task SwitchCommandAsync()
        {
            try
            {
                if (_entry.command.Contains(':'))
                    _defaultPath = new Uri(Path.Combine(_entry.command, "\\"));

                var path = _entry.path;
                var newPath = string.Empty;

                switch (_entry.command)
                {
                    case cm.DISK:
                        await CallOutputAsync();
                        break;
                    case cm.LS:
                        path = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);
                        await CallOutputAsync(path, 10);
                        break;
                    case cm.LS_ALL:
                        path = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);
                        await CallOutputAsync(path, Directory.GetDirectories(path.OriginalString).Length + Directory.GetFiles(path.OriginalString).Length);
                        break;
                    case cm.CP:
                        //path = await TransformAsync(_entry.Remove(0, _entry.command.Length + 1));
                        //path = path.TrimEnd();
                        //newPath = _entry.Remove(0, _entry.command.Length + path.Length + 2) + "\\";
                        //await CallCopyingAsync(path, newPath);
                        break;
                    case cm.CLS:
                        Console.Clear();
                        break;
                    case cm.CD:
                        path = new Uri(Path.Combine(_entry.path.OriginalString, "\\"));
                        _defaultPath = await _validation.CheckEnteredPathAsync(path, _defaultPath);
                        break;
                    case cm.CD_DOT:
                        path = new Uri(_defaultPath.OriginalString.Remove(_defaultPath.OriginalString.Length - 1, 1));
                        _defaultPath = new Uri(Directory.GetParent(path.OriginalString)!.FullName);
                        break;
                    case cm.CD_SLASH:
                        _defaultPath = new Uri(Directory.GetDirectoryRoot(_defaultPath.OriginalString));
                        break;
                    case cm.INFO:
                        await CallInformerAsync(_entry.command);
                        await _displaying.OutputInfoFilesAndDirectoryAsync();
                        break;
                    case cm.HELP:
                        Console.WriteLine(await _repository.GetHelpAsync());
                        break;
                    case cm.RM: 
                        await CallDeletionAsync(); 
                        break;
                }

                if (_entry.command != cm.EXIT)
                {
                    _currentPath.Path = string.Empty;
                    await RunAsync();
                }
                else
                {
                    _currentPath.Path = _defaultPath.OriginalString;
                    await _repository.SavePathAsync(_fileName);
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

        private async Task CallOutputAsync(Uri path, int maxObjects)
            => await _displaying.ViewTreeAsync(path, maxObjects);

        private async Task CallCopyingAsync(string name, Uri newPath)
        {
            var copying = new CopyingService();
            await copying.CopyAsync(_defaultPath, name, newPath);
        }

        private async Task CallDeletionAsync()
        {
            var entry = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);

            var deletion = new RemovingService();

            if (!string.IsNullOrEmpty(Path.GetExtension(entry.OriginalString)))
                deletion.FullPathFile = entry;
            else
                deletion.FullPathDirectory = entry;
        }

        private async Task CallInformerAsync(string command)
        {
            Uri path;

            if (_entry.command.Length == command.Length)
                path = _defaultPath;
            else
                path = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);

            if (!string.IsNullOrEmpty(Path.GetExtension(path.OriginalString)))
            {
                _informer.FullPathFile = path;
                _informer.FullPathDirectory = null;
            }
            else
            {
                _informer.FullPathDirectory = path;
                _informer.FullPathFile = null;
            }
        }
    }
}