namespace ManagerDirectory.Commands
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
			"cls",
			"cd",
			"cd..",
			"cd\\",
			"help",
			"exit"
		};
    }
}
