using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerDirectory.Services;

namespace ManagerDirectory.ConsoleView
{
    internal sealed class Displaying : IDisposable
    {
	    private int _countFiles, _countDirectories;
        private readonly InformingService _informingService;
        private readonly StreamWriter _sw; 

        public Displaying(InformingService informingService)
        {
            _informingService = informingService;
            _sw = new StreamWriter(Console.OpenStandardOutput(), bufferSize: 512, encoding: Encoding.UTF8);
		}

        internal async Task ViewTreeAsync(string path, int maxCountObjects)
	    {
		    var directoryInfo = new DirectoryInfo(path);
		    var length = directoryInfo.Name.Length / 2;
		    int countSpaces;
            var countSelectors = path.Count(symb => symb == '\\');

			if (countSelectors > 2)
				ViewTree(" ~\\" + directoryInfo.Name, directoryInfo.Name.Length / 2 + 2, out countSpaces);
		    else
				ViewTree(" " + path, path.Length - length, out countSpaces);
			
			await Task.Run(() =>
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    if (_countDirectories < maxCountObjects)
                    {
                        _sw.WriteLine($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}{directory.Name}");
                        _countDirectories++;
                    }
                    else
                    {
                        _sw.WriteLine($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
		    _countDirectories = 0;

            await Task.Run(() =>
            {
                foreach (var file in directoryInfo.GetFiles())
                {
                    if (_countFiles < maxCountObjects)
                    {
                        _sw.Write($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        _sw.WriteLine($@"{file.Name}");
                        Console.ResetColor();
                        _countFiles++;
                    }
                    else
                    {
                        _sw.Write($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
			_countFiles = 0;
		}

	    private void ViewTree(string entry, int number, out int countSpaces )
	    {
		    Console.ForegroundColor = ConsoleColor.Yellow;
		    _sw.WriteLine(entry);
		    Console.ResetColor();
		    countSpaces = number;
		}

	    internal void PrintDisks()
	    {
		    var drivers = DriveInfo.GetDrives()
			    .ToList();

		    foreach (var driver in drivers)
			    _sw.WriteLine($@"Имя диска: {driver.Name}");
	    }
             

        internal async Task OutputInfoFilesAndDirectoryAsync() => await Task.Run(() => _sw.WriteLine(_informingService));

        public void Dispose()
        {
	        _sw?.Dispose();
        }
    }
}