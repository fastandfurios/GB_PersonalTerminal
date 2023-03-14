using System;
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

		internal async Task<Uri> CheckEnteredPathAsync(Uri path, Uri defaultPath)
        {
            return await Task.Run(() =>
            {
                if (Directory.Exists(Path.Combine(defaultPath.OriginalString, path.OriginalString)))
                    return new Uri(Path.Combine(defaultPath.OriginalString, path.OriginalString));

                if ((Directory.Exists(path.OriginalString) || File.Exists(path.OriginalString)) && path.OriginalString.Contains('\\'))
                    return path;

                return defaultPath;
			});
        }
	}
}
