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

        public PerforceServerExecutor(ILog logger, string path, string exeSubpath)
            : base(logger, path, exeSubpath, PerforceServerName)
        {
        }

        public bool MakeCheckPoint(string checkpointSubPath)
        {
            var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
            var checkpointLocation = string.IsNullOrWhiteSpace(checkpointSubPath) ? string.Empty : string.Format("{0}/", checkpointSubPath);
            var result = this.ExecuteCommand(string.Format("-r {0} -J {1}journal -jc {1}{2}", this.ExePath, checkpointLocation, currentDate));
            if (result != null && result.Contains(SuccessWord))
            {
                return true;
            }
            else
            {
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
