using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerDirectory.Repository;
using ManagerDirectory.Validation;

namespace ManagerDirectory.IO
{
    public class InputData
    {
        public async Task<string> Input(string defaultPath, Checker checker)
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
