using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerDirectory.Services
{
    internal sealed class CopyingService
    {
        internal async Task CopyAsync(Uri oldPath, string name, Uri newPath)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(name)))
            {
                await Task.Run(() =>
                {
                    Directory.GetFiles(oldPath.OriginalString, name, SearchOption.TopDirectoryOnly).ToList().ForEach(file =>
                        File.Copy(file, file.Replace(oldPath.OriginalString, newPath.OriginalString), true));

                    Console.WriteLine("Копирование прошло успешно!");
                });
            }
            else
            {
                await Task.WhenAll(Task.Run(() =>
                    {
                        Directory.GetDirectories(oldPath.OriginalString, name, SearchOption.TopDirectoryOnly).ToList().ForEach(directory =>
                            Directory.CreateDirectory(directory.Replace(oldPath.OriginalString, newPath.OriginalString)));

                    }),
                    Task.Run(() =>
                    {
                        Directory.GetFiles(oldPath + name, "*.*", SearchOption.TopDirectoryOnly).ToList().ForEach(file =>
                            File.Copy(file, file.Replace(oldPath.OriginalString, newPath.OriginalString), true));

                        Console.WriteLine("Копирование прошло успешно!");
                    }));
            }
        }
    }
}
