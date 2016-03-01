namespace PerforceBackup.ConsoleApp
{
    using System;
    using PerforceBackup.Engine;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Logger;

    public class Program
    {
        public static void Main()
        {
            IResultLogger logger = new ResultLogger(@"Logs\ResultLog.txt");

            try
            {                
                IConfigurations configurations = new Configurations();
                IInfoLogger infoLogger = new ConsoleLogger();

                IJob job = new BackupJob(logger, infoLogger, configurations, Numerics.Version);
                job.Execute();
            }
            catch (Exception ex)
            {
                logger.WriteError(ex);
                Console.WriteLine("ERROR: {0}", ex);
            }
        }
    }
}
