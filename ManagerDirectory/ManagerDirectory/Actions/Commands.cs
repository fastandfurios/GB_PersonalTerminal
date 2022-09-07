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
			"help",
			"exit"
		};
    }
}
