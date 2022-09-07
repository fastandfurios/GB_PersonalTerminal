using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerDirectory.Actions
{
    internal sealed class Copying
    {
	    internal async Task Copy(string oldPath, string name, string newPath)
	    {
		    if (Path.GetExtension(name) != string.Empty)
            {
                await Task.Run(() =>
                {
                    Directory.GetFiles(oldPath, name, SearchOption.TopDirectoryOnly).ToList().ForEach(file =>
                        File.Copy(file, file.Replace(oldPath, newPath), true));
                    
                    Console.WriteLine($"Копирование прошло успешно!");
				});
            }
		    else
            {
                await Task.WhenAll(Task.Run(() =>
                    {
                        Directory.GetDirectories(oldPath, name, SearchOption.TopDirectoryOnly).ToList().ForEach(directory =>
                            Directory.CreateDirectory(directory.Replace(oldPath, newPath)));
                            
                    }),
                    Task.Run(() =>
                    {
                        Directory.GetFiles(oldPath + name, "*.*", SearchOption.TopDirectoryOnly).ToList().ForEach(file => 
                            File.Copy(file, file.Replace(oldPath, newPath), true));

                        Console.WriteLine($"Копирование прошло успешно!");
                    }));
            }
	    }
    }
}
