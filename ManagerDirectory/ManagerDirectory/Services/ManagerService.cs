using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerDirectory.ConsoleView;
using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Properties;
using ManagerDirectory.Validation;
using cm = ManagerDirectory.Commands.Commands;

namespace ManagerDirectory.Services
{
    internal sealed class ManagerService
    {
        #region fields
        private (string command, string path) _entry;
        private string _defaultPath = Resources.DefaultPath;
        private readonly string _fileName = Resources.CurrentPath;
        private readonly string _fileLogErrors = Resources.FileLogErrors;
        private readonly Displaying _displaying;
        private readonly Receiver _receiver;
        private readonly SerializeDeserializeService _sdservice;
        private CurrentPath _currentPath;
        private readonly InformingService _informer;
        private readonly CustomValidation _validation;
        private readonly StartService _startService;
        #endregion

        public ManagerService(Displaying displaying, 
            Receiver receiver, 
            SerializeDeserializeService repository,
            CustomValidation validation,
            InformingService informer,
            CurrentPath currentPath,
            StartService startService)
        {
            (_displaying, _receiver, _sdservice, _validation, _informer, _currentPath, _startService) = 
	            (displaying, receiver, repository, validation, informer, currentPath, startService);
        }
        
        public async Task RunAsync()
        {
	        await _startService.StartAsync(_defaultPath);
	        _currentPath.Path = _startService.GetCurrentPath();

            if (File.Exists(_fileName) && !string.IsNullOrEmpty(_currentPath.Path))
                _defaultPath = _currentPath.Path;
        }

        public async Task SwitchCommandAsync()
        {
	        _entry = await _receiver.ReceiveAsync(_defaultPath);

			try
            {
                if (_entry.command.Contains(':'))
                    _defaultPath = Path.Combine(_entry.command, "\\");

                var path = _entry.path;
                var newPath = string.Empty;

                switch (_entry.command)
                {
                    case cm.DISK:
	                    PrintDisks();
                        break;
                    case cm.LS:
                        path = await _validation.CheckEnteredPathAsync(_defaultPath, _entry.path);
                        await ViewTreeAsync(path, 10);
                        break;
                    case cm.LS_ALL:
	                    path = await _validation.CheckEnteredPathAsync(_defaultPath, _entry.path);
	                    var maxObjects = Directory.EnumerateDirectories(path).Count() + Directory.EnumerateFiles(path).Count();
						await ViewTreeAsync(path, maxObjects);
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
                        path = Path.Combine(_defaultPath, _entry.path);
                        _defaultPath = await _validation.CheckEnteredPathAsync(_defaultPath, path);
                        break;
                    case cm.CD_DOT:
	                    _defaultPath = Directory.GetParent(_defaultPath)!.FullName;
                        break;
                    case cm.CD_SLASH:
                        _defaultPath = Directory.GetDirectoryRoot(_defaultPath);
                        break;
                    case cm.INFO:
                        await CallInformerAsync(_entry.command);
                        await _displaying.OutputInfoFilesAndDirectoryAsync();
                        break;
                    case cm.HELP:
                        Console.WriteLine(await _sdservice.GetHelpAsync());
                        break;
                    case cm.RM: 
                        await CallDeletionAsync(); 
                        break;
                }

                if (_entry.command != cm.EXIT)
                {
	                _currentPath.Path = string.Empty;
                    await SwitchCommandAsync();
                }
                else
                {
                    _currentPath.Path = _defaultPath;
                    await _sdservice.SavePathAsync(_fileName, _currentPath.Path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await File.AppendAllTextAsync(_fileLogErrors, $"{DateTime.Now:G} {e.Message} {e.TargetSite}");
                await File.AppendAllTextAsync(_fileLogErrors, Environment.NewLine);
                await SwitchCommandAsync();
            }
        }

        private void PrintDisks()
        {
	        _displaying.PrintDisks();
        }

        private async Task ViewTreeAsync(string path, int maxObjects)
        {
	        await _displaying.ViewTreeAsync(path, maxObjects);
		}

        private async Task CallCopyingAsync(string name, string newPath)
        {
            var copying = new CopyingService();
            await copying.CopyAsync(_defaultPath, name, newPath);
        }

        private async Task CallDeletionAsync()
        {
            var entry = await _validation.CheckEnteredPathAsync(_defaultPath, _entry.path);

            var deletion = new RemovingService();

            if (!string.IsNullOrEmpty(Path.GetExtension(entry)))
                deletion.FullPathFile = entry;
            else
                deletion.FullPathDirectory = entry;
        }

        private async Task CallInformerAsync(string command)
        {
            string path;

            if (_entry.command.Length == command.Length)
                path = _defaultPath;
            else
                path = await _validation.CheckEnteredPathAsync(_defaultPath, _entry.path);

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
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