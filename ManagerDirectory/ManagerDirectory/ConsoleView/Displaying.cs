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

	    internal async Task ViewTreeAsync(Uri path, int maxCountObjects)
	    {
		    var directoryInfo = new DirectoryInfo(path.OriginalString);
		    var length = directoryInfo.Name.Length / 2;
		    int countSpaces;
            var countSelectors = path.OriginalString.Count(symb => symb == '\\');

			if (countSelectors > 2)
				ViewTree(" ~\\" + directoryInfo.Name, directoryInfo.Name.Length / 2 + 2, out countSpaces);
		    else
				ViewTree(" " + path.OriginalString, path.OriginalString.Length - length, out countSpaces);
			
			await Task.Run(() =>
            {
                foreach (var directory in directoryInfo.GetDirectories())
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
                foreach (var file in directoryInfo.GetFiles())
                {
                    if (_countFiles < maxCountObjects)
                    {
                        Console.Write($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($@"{file.Name}");
                        Console.ResetColor();
                        _countFiles++;
                    }
                    else
                    {
                        Console.Write($@"{new string(' ', countSpaces)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
			_countFiles = 0;
		}

	    private void ViewTree(string entry, int number, out int countSpaces )
	    {
		    Console.ForegroundColor = ConsoleColor.Yellow;
		    Console.WriteLine(entry);
		    Console.ResetColor();
		    countSpaces = number;
		}
		
	    internal async Task GetDisksAsync()
            => await Task.Run(() => DriveInfo.GetDrives()
                .ToList()
                .ForEach(drive => Console.WriteLine($@"Имя диска: {drive.Name}")));

        internal async Task OutputInfoFilesAndDirectoryAsync(InformingService informer) => await Task.Run(() => Console.WriteLine(informer));
    }
}