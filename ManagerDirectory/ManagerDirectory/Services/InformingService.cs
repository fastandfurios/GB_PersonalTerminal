using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerDirectory.Enums;

namespace ManagerDirectory.Services
{
    public class InformingService
    {
        private string _fullPathFile;
        public string FullPathFile
        {
            get => _fullPathFile;
            set => _fullPathFile = value;
        }

        private string _fullPathDirectory;
        public string FullPathDirectory
        {
            get => _fullPathDirectory;
            set => _fullPathDirectory = value;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_fullPathDirectory) && Path.GetExtension(_fullPathDirectory) == string.Empty)
            {
                var directoryInfo = new DirectoryInfo(_fullPathDirectory);
                var countDirectory = directoryInfo.GetDirectories("*", SearchOption.AllDirectories).Length;
                var countFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Length;
                long size = 0;

                directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(file => size += file.Length);

                return $"Количество папок: {countDirectory}\n" +
                       $"Количество файлов: {countFiles}\n" +
                       $"Размер: {ConvertAsync(size).GetAwaiter().GetResult()}";
            }
            else
            {
                var fileInfo = new FileInfo(_fullPathFile);

                return $"Имя: {Path.GetFileNameWithoutExtension(_fullPathFile)}\n" +
                       $"Расширение: {fileInfo.Extension}\n" +
                       $"Размер: {ConvertAsync(fileInfo.Length).GetAwaiter().GetResult()}";
            }
        }

        private async Task<string> ConvertAsync(long size)
        {
            return await Task.Run(async () =>
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
            });
        }
    }
}
