using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerDirectory.Actions;

namespace ManagerDirectory.IO
{
    public class Output
    {
	    private int _countFiles, _countDirectory;

	    public async Task OutputTree(string path, int maxObjects)
	    {
		    var directoryInfo = new DirectoryInfo(path);
		    int length = await Task.Run(() => directoryInfo.Name.Length / 2);
		    int spaceLength;
		    var arraySelector = await Task.Run(() => path.Where(s => s == '\\').ToList());

			if (arraySelector.Count > 2)
				OutputTree(" ~\\" + directoryInfo.Name, arraySelector, directoryInfo.Name.Length / 2 + 2, out spaceLength);
		    else
				OutputTree(" " + path, arraySelector, path.Length - length, out spaceLength);
			
			await Task.Run(() =>
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    if (_countDirectory < maxObjects)
                    {
                        Console.WriteLine($"{new string(' ', spaceLength)}|{new string('-', length + 1)}{directory.Name}");
                        _countDirectory++;
                    }
                    else
                    {
                        Console.WriteLine($"{new string(' ', spaceLength)}|{new string('-', length + 1)}...");
                        break;
                    }
                }
			});
            
		    _countDirectory = 0;

            await Task.Run(() =>
            {
                foreach (var file in directoryInfo.GetFiles())
                {
                    if (_countFiles < maxObjects)
                    {
                        Console.Write($"{new string(' ', spaceLength)}|{new string('-', length + 1)}");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{file.Name}\n");
                        Console.ResetColor();
                        _countFiles++;
                    }
                    else
                    {
                        Console.Write($"{new string(' ', spaceLength)}|{new string('-', length + 1)}...\n");
                        break;
                    }
                }
			});
            
			_countFiles = 0;
		}

	    private void OutputTree(string str, List<char> arraySelector, int exp, out int spaceLength )
	    {
		    Console.ForegroundColor = ConsoleColor.Yellow;
		    Console.WriteLine(str);
		    Console.ResetColor();
		    spaceLength = exp;
		    arraySelector.RemoveRange(0, arraySelector.Count);
		}
		
	    public async Task GetDrives()
        {
            await Task.Run(() =>
            {
                DriveInfo.GetDrives().ToList().ForEach(drive => Console.WriteLine($"Имя диска: {drive.Name}"));
            });
        }

	    public async Task OutputInfoFilesAndDirectory(Informer informer) => await Task.Run(() => Console.WriteLine(informer));
    }
}
