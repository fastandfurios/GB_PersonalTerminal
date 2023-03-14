namespace ManagerDirectory.Commands
{
	public static class Commands
    {
        /// <summary> Команда для смены диска </summary>
        public const string DISK = "disk";
        /// <summary> Команда для отображения дерева каталога, но не более 10 объектов </summary>
        public const string LS = "ls";
        /// <summary> Команда для отображения всего дерева каталога </summary>
        public const string LS_ALL = "lsAll";
        /// <summary> Команда для копирование каталогов и файлов </summary>
        public const string CP = "cp";
        /// <summary> Команда для удаления каталогов и файлов </summary>
        public const string RM = "rm";
        /// <summary> Команда для отображения информации о каталоге или файле </summary>
        public const string INFO = "info";
        /// <summary> Команда для очистки консоли </summary>
        public const string CLS = "cls";
        /// <summary> Компанда для перехода по каталогам </summary>
        public const string CD = "cd";
        /// <summary> Команда для перехода к родительскому каталогу </summary>
        public const string CD_DOT = "cd..";
        /// <summary> Команда для перехода к главному каталогу </summary>
        public const string CD_SLASH = "cd\\";
        /// <summary> Команда для вызова помощи </summary>
        public const string HELP = "help";
        /// <summary> Команда для выхода из программы </summary>
        public const string EXIT = "exit";
    }
}
