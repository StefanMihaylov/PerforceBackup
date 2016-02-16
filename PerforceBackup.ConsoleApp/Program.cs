namespace PerforceBackup.ConsoleApp
{
    using System;
    using System.Configuration;
    using System.IO;

    using PerforceBackup.Engine;
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Excel;
    using PerforceBackup.Engine.Models;
    using PerforceBackup.Engine.Logger;
    using log4net;

    public class Program
    {
        public const string LogArhiveName = "Backup_logs";
        public const string CheckpointArhiveName = "Backup_ckp";
        public const string CheckpointLogName = "Checkpoint_Log";
        public const string CheckpointLogExtension = "xls";
        public const string Log4NetPath = @"Logs\ResultLog.txt";

        private static ILog logger;

        public static void Main()
        {
            try
            {                
                logger = LogManager.GetLogger("PerforceBackup");
                Logger.Setup(Log4NetPath);

                var checkpointLogInfo = new CheckPointLogModel();

                Console.WriteLine(" - Start...");
                logger.Info("PerforceBackup started...");

                checkpointLogInfo.StartDate = DateTime.Now;
                Console.WriteLine(" - Date: {0}", checkpointLogInfo.StartDate.ToString(StringConstrants.DateTimeFormat));

                // validate
                var rootPath = ConfigurationManager.AppSettings["RootPath"];
                var executor = new PerforceCommandLineExecutor(logger, rootPath, string.Empty);
                Console.WriteLine(" - Validate: {0}", executor.ValidateCommand() ? "OK" : "NO");

                // Checkpoint
                 var server = new PerforceServerExecutor(logger, rootPath, ConfigurationManager.AppSettings["ServerSubPath"]);
                // /*
                 Console.WriteLine(" - Stop Server: {0}", executor.StopServerCommand() ? "Done" : "NO");                
                 Console.WriteLine(" - Checkpoint: {0}", server.MakeCheckPoint(ConfigurationManager.AppSettings["CheckpointSubPath"]) ? "Done" : "NO");
                // */

                 // Backuped Lod and Auditlig Filed
                 var backupArhiveSubPath = ConfigurationManager.AppSettings["BackupArhiveSubPath"];
                 var sevenZip = new SevenZip(logger, rootPath, ConfigurationManager.AppSettings["SevenZipSubPath"]);
                 var logArhivator = new LogFileArhivator(sevenZip, rootPath, ConfigurationManager.AppSettings["LogFilesSubPath"]);
                 string LogArhivePath = Path.Combine(rootPath, backupArhiveSubPath);
                 var logMaxSize = LogFileArhivator.GetMaxSize(ConfigurationManager.AppSettings["MaxLogSize"]);
                 var auditogMaxSize = LogFileArhivator.GetMaxSize(ConfigurationManager.AppSettings["MaxAuditLogSize"]);
                 checkpointLogInfo.Log = logArhivator.Compress("log", logMaxSize, LogArhivePath, LogArhiveName);
                 checkpointLogInfo.AuditLog = logArhivator.Compress("auditlog", auditogMaxSize, LogArhivePath, LogArhiveName);
                 Console.WriteLine(" - Log: {0}", checkpointLogInfo.Log);
                 Console.WriteLine(" - AuditLog: {0}", checkpointLogInfo.AuditLog);

                 // /*
                 // Remove old Checkpoints & make Arhive
                 var checkpointBackup = new CheckpointArhivator(sevenZip, rootPath, ConfigurationManager.AppSettings["JournalSubPath"]);
                 Console.Write(" - Checkpoints Arhive: ");
                 var checkpoint = checkpointBackup.Compress(LogArhivePath, CheckpointArhiveName);
                 Console.WriteLine("{0}", checkpoint.IsOldCheckpointsCompressed ? "Done" : "NO"); 
                
                 var rootBackup = new RootArhivator(sevenZip, rootPath);
                 Console.Write(" - Root Arhive: ");
                 checkpointLogInfo.Arhive = rootBackup.Compress(checkpoint, ConfigurationManager.AppSettings["ArhivePath"], ConfigurationManager.AppSettings["ArhiveName"], ConfigurationManager.AppSettings["ArhiveType"]);
                 Console.WriteLine("{0}", checkpointLogInfo.Arhive);

                 // Start Server & end Backup; Statistics
                 var cmd = new CommandPromptExecutor(logger);
                 Console.WriteLine(" - Start Server: {0}", cmd.StartService("Perforce", "successfully") ? "Done" : "NO"); // */

                checkpointLogInfo.Counters = executor.CountersCommand();
                Console.WriteLine(" - Counters: {0}", checkpointLogInfo.Counters);

                checkpointLogInfo.Sizes = executor.SizeCommand();
                Console.WriteLine(" - Sizes: {0}", checkpointLogInfo.Sizes);

                var dir = new DirectoryInformation();
                checkpointLogInfo.DepotSize = dir.DirSizeInMb(rootPath, ConfigurationManager.AppSettings["DepotSubPath"]);
                Console.WriteLine(" - Depot Size: {0:0.00}Mb ", checkpointLogInfo.DepotSize);

                checkpointLogInfo.Users = executor.UsersCountCommand();
                Console.WriteLine(" - Users: {0}", checkpointLogInfo.Users);

                checkpointLogInfo.ServerInfo = server.GetServerVersion();
                Console.WriteLine(" - Server: {0}", checkpointLogInfo.ServerInfo);

                // Write to Excel
                var excelWriter = new ExcelWriter(Path.Combine(rootPath, backupArhiveSubPath), CheckpointLogName, CheckpointLogExtension);
                excelWriter.AddRow(checkpointLogInfo);

                rootBackup.AddFileToArhiv(excelWriter.FullFileRoot, checkpointLogInfo.Arhive.ArhiveFullPath);

                logger.Info("Data saved to Excel");

                logger.Info("End" + Environment.NewLine);
                Console.WriteLine(" - End");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                Console.WriteLine("ERROR: {0}", ex);
            }
        }
    }
}
