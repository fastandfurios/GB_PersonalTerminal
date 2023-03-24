using System.IO;
using System.Threading.Tasks;
using cm = ManagerDirectory.Commands.Commands;

namespace ManagerDirectory.Validation
{
    public sealed class CustomValidation
    {
        internal async Task<bool> CheckForCommandAsync(string cmd)
        {
            return await Task.Run(() =>
            {
                if ((cm.CD.Equals(cmd) || cm.CD_DOT.Equals(cmd)
                        || cm.CD_SLASH.Equals(cmd) || cm.CLS.Equals(cmd)
                        || cm.CP.Equals(cmd) || cm.DISK.Equals(cmd)
                        || cm.EXIT.Equals(cmd) || cm.HELP.Equals(cmd)
                        || cm.INFO.Equals(cmd) || cm.LS.Equals(cmd)
                        || cm.LS_ALL.Equals(cmd) || cm.RM.Equals(cmd)
                        && !string.IsNullOrEmpty(cmd)) 
                        || cmd.Contains(':'))

                        return true;

                return false;
			});
        }

		internal async Task<string> CheckEnteredPathAsync(string defaultPath, string path)
        {
            return await Task.Run(() =>
            {
                if (Directory.Exists(Path.Combine(defaultPath, path)))
                    return Path.Combine(defaultPath, path);

                if ((Directory.Exists(path) || File.Exists(path)) && path.Contains('\\'))
                    return path;

                return defaultPath;
			});
        }
	}
}
