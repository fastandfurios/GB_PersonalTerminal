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
	    private string _entry;

	    public async Task<string> Input(string defaultPath, Checker checker)
        {
            return await Task.Run(async () =>
            {
                var flag = false;

                do
                {
                    Console.Write($"{defaultPath} > ");
                    _entry = Console.ReadLine();
                    flag = await checker.CheckInputCommand(_entry);
                } while (!flag);

                return _entry;
			});
        }
    }
}
