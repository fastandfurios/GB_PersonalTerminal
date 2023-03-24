using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerDirectory.Services;

namespace ManagerDirectory.ConsoleView
{
    internal sealed class Displaying
    {
	    private int _countFiles, _countDirectories;
        private readonly InformingService _informingService; 

        public Displaying(InformingService informingService)
        {
            _informingService = informingService;
		}

        internal async Task ViewTreeAsync(string path, int maxCountObjects)
        {
	        var directoryInfo = new DirectoryInfo(path);
		    var length = directoryInfo.Name.Length / 2;
		    int countSpaces;
            var countSelectors = path.Count(slash => slash == '\\');

			if (countSelectors > 2)
				ViewTree(" ~\\" + directoryInfo.Name, directoryInfo.Name.Length / 2 + 2, out countSpaces);
		    else
				ViewTree(" " + path, path.Length - length, out countSpaces);
			
			await Task.Run(() =>
            {
                foreach (var directory in directoryInfo.EnumerateDirectories())
                {
                    if (_countDirectories < maxCountObjects)
                    {
                        Console.WriteLine($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}{directory.Name}");
                        _countDirectories++;
                    }
                    else
                    {
                        Console.WriteLine($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
		    _countDirectories = 0;

            await Task.Run(() =>
            {
                foreach (var file in directoryInfo.EnumerateFiles())
                {
                    if (_countFiles < maxCountObjects)
                    {
                        Console.Write($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}");
                        Console.WriteLine($@"{file.Name}");
                        _countFiles++;
                    }
                    else
                    {
                        Console.WriteLine($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
			_countFiles = 0;
		}

	    private void ViewTree(string entry, int middleLine, out int countSpaces )
	    {
		    Console.ForegroundColor = ConsoleColor.DarkYellow;
		    Console.WriteLine(entry);
            Console.ResetColor();
		    countSpaces = middleLine;
		}

	    internal void PrintDisks()
	    {
		    var drivers = DriveInfo.GetDrives()
			    .ToList();

		    foreach (var driver in drivers)
			    Console.WriteLine($@"Имя диска: {driver.Name}");
	    }
             

        internal async Task OutputInfoFilesAndDirectoryAsync() => await Task.Run(() => Console.WriteLine(_informingService));
    }
}