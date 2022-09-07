using System;
using System.Threading.Tasks;
using ManagerDirectory.Validation;

namespace ManagerDirectory.IO
{
    internal sealed class InputData
    {
        internal async Task<string> Input(string defaultPath, Checker checker)
        {
            return await Task.Run(async () =>
            {
                var flag = false;
                var entry = string.Empty;

                do
                {
                    Console.Write($"{defaultPath} > ");
                    entry = Console.ReadLine();
                    flag = await checker.CheckInputCommand(entry);
                } while (!flag);

                return entry;
			});
        }
    }
}
