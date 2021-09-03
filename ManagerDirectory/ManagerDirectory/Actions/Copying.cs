using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerDirectory.Actions
{
    public class Copying
    {
	    public async Task Copy(string oldPath, string name, string newPath)
	    {
		    if (Path.GetExtension(name) != string.Empty)
            {
                await Task.Run(() =>
                {
                    foreach (var file in Directory.GetFiles(oldPath, name, SearchOption.TopDirectoryOnly))
                        File.Copy(file, file.Replace(oldPath, newPath), true);

                    Console.WriteLine($"Копирование прошло успешно!");
				});
            }
		    else
            {
                await Task.WhenAll(Task.Run(() =>
                    {
                        foreach (var directory in
                            Directory.GetDirectories(oldPath, name, SearchOption.TopDirectoryOnly))
                            Directory.CreateDirectory(directory.Replace(oldPath, newPath));
                    }),
                    Task.Run(() =>
                    {
                        foreach (var file in Directory.GetFiles(oldPath + name, "*.*", SearchOption.TopDirectoryOnly))
                            File.Copy(file, file.Replace(oldPath, newPath), true);

                        Console.WriteLine($"Копирование прошло успешно!");
                    }));
            }
	    }
    }
}
