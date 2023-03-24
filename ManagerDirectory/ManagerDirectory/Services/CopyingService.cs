using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerDirectory.Services
{
    internal sealed class CopyingService
    {
        internal async Task CopyAsync(string oldPath, string name, string newPath)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(name)))
            {
                await Task.Run(() =>
                {
                    Directory.EnumerateFiles(oldPath, name, SearchOption.TopDirectoryOnly).ToList().ForEach(file =>
                        File.Copy(file, file.Replace(oldPath, newPath), true));

                    Console.WriteLine("Копирование прошло успешно!");
                });
            }
            else
            {
                await Task.WhenAll(
                    Task.Run(() =>
                    {
                        Directory.EnumerateDirectories(oldPath, name, SearchOption.TopDirectoryOnly).ToList().ForEach(directory =>
                            Directory.CreateDirectory(directory.Replace(oldPath, newPath)));

                    }),
                    Task.Run(() =>
                    {
                        Directory.EnumerateFiles(oldPath + name, "*.*", SearchOption.TopDirectoryOnly).ToList().ForEach(file =>
                            File.Copy(file, file.Replace(oldPath, newPath), true));

                        Console.WriteLine("Копирование прошло успешно!");
                    }));
            }
        }
    }
}
