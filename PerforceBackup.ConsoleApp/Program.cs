namespace PerforceBackup.ConsoleApp
{
    using System;
    using log4net;
    using PerforceBackup.Engine;
    using PerforceBackup.Engine.ApiCommands;
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Logger;
    using PerforceBackup.Engine.Models;

    public class Program
    {
        public const string LoggerName = "PerforceBackup";
        public const string Log4NetPath = @"Logs\ResultLog.txt";

        public static void Main()
        {
            ILog logger = LogManager.GetLogger(LoggerName);
            Logger.Setup(Log4NetPath);

            try
            {
                IConfigurations configurations = new Configurations();
                IInfoLogger infoLogger = new ConsoleLogger();

                IJob job = new BackupJob(logger, infoLogger, configurations, Numerics.Version);
                job.Execute();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                Console.WriteLine("ERROR: {0}", ex);
            }
        }
    }
}
