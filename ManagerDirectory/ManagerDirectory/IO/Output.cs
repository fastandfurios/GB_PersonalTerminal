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
	    private DirectoryInfo _directory;
	    private int _countFiles, _countDirectory;

	    /// <summary>
		/// Выводит список директорий и файлов
		/// </summary>
		/// <param name="path">Путь</param>
	    public async Task OutputTree(string path, int maxObjects)
	    {
		    _directory = new DirectoryInfo(path);
		    int length = await Task.Run(() => _directory.Name.Length / 2);
		    int spaceLength;
		    var arraySelector = await Task.Run(() => path.Where(s => s == '\\').ToList());

			if (arraySelector.Count > 2)
				OutputTree(" ~\\" + _directory.Name, arraySelector, _directory.Name.Length / 2 + 2, out spaceLength);
		    else
				OutputTree(" " + path, arraySelector, path.Length - length, out spaceLength);
			
			foreach (var directory in _directory.GetDirectories())
		    {
			    if (_countDirectory < maxObjects)
			    {
				    Console.WriteLine(
					    $"{new string(' ', spaceLength)}|{new string('-', length + 1)}{directory.Name}");
				    _countDirectory++;
			    }
			    else
			    {
				    Console.WriteLine(
					    $"{new string(' ', spaceLength)}|{new string('-', length + 1)}...");
				    break;
			    }
		    }

		    _countDirectory = 0;

			foreach (var file in _directory.GetFiles())
		    {
			    if (_countFiles < maxObjects)
			    {
				    Console.Write(
					    $"{new string(' ', spaceLength)}|{new string('-', length + 1)}");
				    Console.ForegroundColor = ConsoleColor.DarkGreen;
				    Console.Write($"{file.Name}\n");
				    Console.ResetColor();
				    _countFiles++;
			    }
			    else
			    {
				    Console.Write(
					    $"{new string(' ', spaceLength)}|{new string('-', length + 1)}...\n");
				    break;
			    }
		    }

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

		/// <summary>
		/// Получает список доступных дисков в системе
		/// </summary>
	    public async Task GetDrives()
        {
            await Task.Run(() =>
            {
                var drives = DriveInfo.GetDrives();

                foreach (var drive in drives)
                    Console.WriteLine($"Имя диска: {drive.Name}");
			});
        }

	    public void OutputInfoFilesAndDirectory(Informer informer) => Console.WriteLine(informer);
    }
}
