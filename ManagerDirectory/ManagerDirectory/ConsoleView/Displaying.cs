using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerDirectory.Services;

namespace ManagerDirectory.ConsoleView
{
    internal sealed class Displaying
    {
	    private int _countFiles, _countDirectory;

	    internal async Task ViewTreeAsync(Uri path, int maxCountObjects)
	    {
		    var directoryInfo = new DirectoryInfo(path.OriginalString);
		    var length = directoryInfo.Name.Length / 2;
		    int countSpace;
            var countSelectors = path.OriginalString.Count(symb => symb == '\\');

			if (countSelectors > 2)
				ViewTree(" ~\\" + directoryInfo.Name, directoryInfo.Name.Length / 2 + 2, out countSpace);
		    else
				ViewTree(" " + path.OriginalString, path.OriginalString.Length - length, out countSpace);
			
			await Task.Run(() =>
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    if (_countDirectory < maxCountObjects)
                    {
                        Console.WriteLine($@"{new string(' ', countSpace)}|{new string('-', length + 1)}{directory.Name}");
                        _countDirectory++;
                    }
                    else
                    {
                        Console.WriteLine($@"{new string(' ', countSpace)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
		    _countDirectory = 0;

            await Task.Run(() =>
            {
                foreach (var file in directoryInfo.GetFiles())
                {
                    if (_countFiles < maxCountObjects)
                    {
                        Console.Write($@"{new string(' ', countSpace)}|{new string('-', length + 1)}");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($@"{file.Name}");
                        Console.ResetColor();
                        _countFiles++;
                    }
                    else
                    {
                        Console.Write($@"{new string(' ', countSpace)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
			_countFiles = 0;
		}

	    private void ViewTree(string str, int exp, out int spaceLength )
	    {
		    Console.ForegroundColor = ConsoleColor.Yellow;
		    Console.WriteLine(str);
		    Console.ResetColor();
		    spaceLength = exp;
		}
		
	    internal async Task GetDisksAsync()
            => await Task.Run(() => DriveInfo.GetDrives()
                .ToList()
                .ForEach(drive => Console.WriteLine($@"Имя диска: {drive.Name}")));

        internal async Task OutputInfoFilesAndDirectoryAsync(InformingService informer) => await Task.Run(() => Console.WriteLine(informer));
    }
}