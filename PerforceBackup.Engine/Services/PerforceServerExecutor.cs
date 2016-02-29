namespace PerforceBackup.Engine.Services
{
    using log4net;
    using PerforceBackup.Engine.Base;
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Models;
    using PerforceBackup.Engine.Interfaces;
    using System;
    using System.Linq;

    public class PerforceServerExecutor : CommandExecutor, IPerforceServerExecutor
    {
        public const string PerforceServerName = "p4d.exe";
        public const string SuccessWord = "Rotating";

        public PerforceServerExecutor(ILog logger, IInfoLogger infoLogger, string path, string exeSubpath)
            : base(logger, path, exeSubpath, PerforceServerName)
        {
            this.InfoLogger = infoLogger;
        }

        protected IInfoLogger InfoLogger { get; private set; }

        public bool MakeCheckPoint(string checkpointSubPath)
        {
            this.InfoLogger.Write(" - Checkpoint: ");

            var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
            string checkpointLocation = string.Empty;
            if (!string.IsNullOrWhiteSpace(checkpointSubPath))
            {
                checkpointLocation = string.Format("{0}/", checkpointSubPath);
            }
            string command = string.Format("-r {0} -J {1}journal -jc {1}{2}", this.ExePath, checkpointLocation, currentDate);

            var result = this.ExecuteCommand(command);
            if (result != null && result.Contains(SuccessWord))
            {
                this.InfoLogger.WriteLine(StringConstrants.SuccessMessage);
                return true;
            }
            else
            {
                this.InfoLogger.WriteLine(StringConstrants.ErrorMessage);
                throw new InvalidOperationException(result);
            }
        }

        public ServerVersionModel GetServerVersion()
        {
            var commandResult = this.ExecuteCommand("-V");
            var commandResultAsRows = commandResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var line = commandResultAsRows.Where(r => r.Contains("Rev.")).ToList();

            if (line.Count != 1)
            {
                throw new InvalidOperationException(commandResult);
            }

            var lineAsArray = line.First().Split(new char[] { ' ', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new ServerVersionModel()
             {
                 Platform = lineAsArray[2],
                 Version = lineAsArray[3],
                 Revision = lineAsArray[4],
                 Date = new DateTime(int.Parse(lineAsArray[5]), int.Parse(lineAsArray[6]), int.Parse(lineAsArray[7]))
             };

            return result;
        }
    }
}
