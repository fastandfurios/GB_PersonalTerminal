using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerDirectory.Actions
{
	readonly struct Commands
	{
		public string[] ArrayCommands => new string[]
		{
			"disk",
			"ls",
			"lsAll",
			"cp",
			"rm",
			"info",
			"clear",
			"cd",
			"cd..",
			"cd\\",
			"exit"
		};
    }
}
