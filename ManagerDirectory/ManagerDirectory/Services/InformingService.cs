using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerDirectory.Enums;

namespace ManagerDirectory.Services
{
    public class InformingService
    {
        private Uri _fullPathFile;
        public Uri FullPathFile
        {
            get => _fullPathFile;
            set => _fullPathFile = value;
        }

        private Uri _fullPathDirectory;
        public Uri FullPathDirectory
        {
            get => _fullPathDirectory;
            set => _fullPathDirectory = value;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_fullPathDirectory.OriginalString) && Path.GetExtension(_fullPathDirectory.OriginalString) == string.Empty)
            {
                var directoryInfo = new DirectoryInfo(_fullPathDirectory.OriginalString);
                var countDirectory = directoryInfo.EnumerateDirectories("*", SearchOption.AllDirectories).Count();
                var countFiles = directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Count();
                long size = 0;

                directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories)
                    .ToList()
                    .ForEach(file => size += file.Length);

                return $"Количество папок: {countDirectory}\n" +
                       $"Количество файлов: {countFiles}\n" +
                       $"Размер: {ConvertAsync(size).GetAwaiter().GetResult()}";
            }
            else
            {
                var fileInfo = new FileInfo(_fullPathFile.OriginalString);

                return $"Имя: {Path.GetFileNameWithoutExtension(_fullPathFile.OriginalString)}\n" +
                       $"Расширение: {fileInfo.Extension}\n" +
                       $"Размер: {ConvertAsync(fileInfo.Length).GetAwaiter().GetResult()}";
            }
        }

        private async Task<string> ConvertAsync(long size)
        {
            return size switch
            {
                < 1024 => $"{size.ToString()} {Value.B.ToString()}",
                > 1024 and < 1_048_576 => await Task.Run(() => $"{(double)size / 1024:F} {Value.KB.ToString()}"),
                > 1_048_576 and < 1_073_741_824 => await Task.Run(() =>
                    $"{(double)size / 1_048_576:F} {Value.MB.ToString()}"),
                > 1_073_741_824 => await Task.Run(() =>
                    $"{(double)size / 1_073_741_824:F} {Value.GB.ToString()}"),
                _ => "0"
            };
        }
    }
}
