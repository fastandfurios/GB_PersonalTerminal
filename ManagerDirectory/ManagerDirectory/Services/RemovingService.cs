using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerDirectory.Services
{
    public class RemovingService
    {
        private int _countFiles;
        private int _countDirectory;
        private Uri _fullPathDirectory;
        private Uri _fullPathFile;

        public Uri FullPathDirectory
        {
            get => _fullPathDirectory;
            set
            {
                _fullPathDirectory = value;

                DeleteAsync().GetAwaiter();

                Directory.Delete(_fullPathDirectory.OriginalString);
                Console.WriteLine("Удаление прошло успешно!");
            }
        }

        public Uri FullPathFile
        {
            get => _fullPathFile;
            set
            {
                _fullPathFile = value;

                File.Delete(_fullPathFile.OriginalString);
                Console.WriteLine("Удаление прошло успешно!");
            }
        }

        private async Task DeleteAsync()
        {
            _countFiles = Directory.EnumerateFiles(_fullPathDirectory.OriginalString, "*.*", SearchOption.AllDirectories).Count();
            _countDirectory = Directory.EnumerateDirectories(_fullPathDirectory.OriginalString, "*", SearchOption.AllDirectories).Count();

            if (_countFiles != 0)
            {
                File.Delete(Directory.GetFiles(_fullPathDirectory.OriginalString, "*.*", SearchOption.AllDirectories)[_countFiles - 1]);
                await DeleteAsync();
            }

            if (_countDirectory != 0)
            {
                Directory.Delete(Directory.GetDirectories(_fullPathDirectory.OriginalString, "*", SearchOption.AllDirectories)[_countDirectory - 1]);
                await DeleteAsync();
            }

        }
    }
}
