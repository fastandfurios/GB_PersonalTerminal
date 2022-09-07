using System;
using System.IO;
using System.Threading.Tasks;

namespace ManagerDirectory.Services
{
    public class RemovingService
    {
        private int _countFiles;
        private int _countDirectory;
        private string _fullPathDirectory;
        private string _fullPathFile;

        public string FullPathDirectory
        {
            get => _fullPathDirectory;
            set
            {
                _fullPathDirectory = value;

                DeleteAsync().GetAwaiter();

                Directory.Delete(_fullPathDirectory);
                Console.WriteLine("Удаление прошло успешно!");
            }
        }

        public string FullPathFile
        {
            get => _fullPathFile;
            set
            {
                _fullPathFile = value;

                File.Delete(_fullPathFile);
                Console.WriteLine("Удаление прошло успешно!");
            }
        }

        private async Task DeleteAsync()
        {
            _countFiles = Directory.GetFiles(_fullPathDirectory, "*.*", SearchOption.AllDirectories).Length;
            _countDirectory = Directory.GetDirectories(_fullPathDirectory, "*", SearchOption.AllDirectories).Length;

            if (_countFiles != 0)
            {
                File.Delete(Directory.GetFiles(_fullPathDirectory, "*.*", SearchOption.AllDirectories)[_countFiles - 1]);
                await DeleteAsync();
            }

            if (_countDirectory != 0)
            {
                Directory.Delete(Directory.GetDirectories(_fullPathDirectory, "*", SearchOption.AllDirectories)[_countDirectory - 1]);
                await DeleteAsync();
            }

        }
    }
}
