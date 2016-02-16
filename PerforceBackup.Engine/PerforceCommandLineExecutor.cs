namespace PerforceBackup.Engine
{
    using log4net;
    using PerforceBackup.Engine.Models;
    using System;
    using System.Diagnostics;
    using System.Text;

    public class PerforceCommandLineExecutor : CommandExecutor
    {
        public const string PerforceCommandClientName = "p4.exe";

        public PerforceCommandLineExecutor(ILog logger, string path, string exeSubpath)
            : base(logger, path, exeSubpath, PerforceCommandClientName)
        {
        }

        public bool StopServerCommand()
        {
            var result = this.ExecuteCommand("admin", "stop");
            if (!string.IsNullOrWhiteSpace(result))
            {
                throw new OperationCanceledException(result);
            }

            return true;
        }

        public bool ValidateCommand()
        {
            var result = this.ExecuteCommand("verify", "-q //...");
            if (!string.IsNullOrWhiteSpace(result))
            {
                throw new OperationCanceledException(result);
            }

            return true;
        }

        public CountersModel CountersCommand()
        {
            var stdOut = this.ExecuteCommand("counters");
            var splitedResult = stdOut.Split(new char[] { '=', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new CountersModel();
            for (int i = 0; i < splitedResult.Length; i++)
            {
                var part = splitedResult[i];
                switch (part.ToLower().Trim())
                {
                    case "change":
                        result.Change = long.Parse(splitedResult[i + 1]);
                        i++;
                        break;
                    case "journal":
                        result.Journal = long.Parse(splitedResult[i + 1]);
                        i++;
                        break;
                    case "upgrade":
                        result.Upgrade = long.Parse(splitedResult[i + 1]);
                        i++;
                        break;
                    case "maxcommitchange":
                        result.MaxCommitChange = long.Parse(splitedResult[i + 1]);
                        i++;
                        break;
                }
            }

            return result;
        }

        public SizesModel SizeCommand()
        {
            var lastVersions = this.ExecuteCommand("sizes", "-z //depot/...");
            var allVersions = this.ExecuteCommand("sizes", "-z -a //depot/...");

            var result = new SizesModel();

            var stdoutParts = lastVersions.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            result.FilesCount = long.Parse(stdoutParts[1]);
            result.FileSize = long.Parse(stdoutParts[3]);

            stdoutParts = allVersions.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            result.RevisionsCount = long.Parse(stdoutParts[1]);
            result.RevisionsSize = long.Parse(stdoutParts[3]);

            return result;
        }

        public int UsersCountCommand()
        {
            var stdOut = this.ExecuteCommand("users");
            var splittedResult = stdOut.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return splittedResult.Length;
        }
    }
}
