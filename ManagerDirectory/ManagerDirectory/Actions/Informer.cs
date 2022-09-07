using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerDirectory.Actions
{
    public class Informer
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
				int countDirectory = directoryInfo.GetDirectories("*", SearchOption.AllDirectories).Length;
			    int countFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Length;
			    long size = 0;

				directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(file => size += file.Length);

			    return $"Количество папок: {countDirectory}\n" +
			           $"Количество файлов: {countFiles}\n" +
			           $"Размер: {Converter(size).GetAwaiter().GetResult()}";
		    }
		    else
		    {
			    var fileInfo = new FileInfo(_fullPathFile);

				return $"Имя: {Path.GetFileNameWithoutExtension(_fullPathFile)}\n" +
			           $"Расширение: {fileInfo.Extension}\n" +
			           $"Размер: {Converter(fileInfo.Length).GetAwaiter().GetResult()}";
		    }
	    }

		private async Task<string> Converter(long size)
        {
            return await Task.Run(async () =>
            {
                if (size < 1024)
                    return $"{size.ToString()} B";

                if (1024 < size && size < 1_048_576)
                    return await Task.Run(() => $"{((double)size / 1024).ToString("F")} KB");

                if (1_048_576 < size && size < 1_073_741_824)
                    return await Task.Run(() => $"{((double)size / 1_048_576).ToString("F")} MB");

                if (size > 1_073_741_824)
                    return await Task.Run(() => $"{((double)size / 1_073_741_824).ToString("F")} GB");

                return default;
			});
        }
    }
}
