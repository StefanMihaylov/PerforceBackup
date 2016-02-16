namespace PerforceBackup.ConsoleApp
{
    using log4net;
    using PerforceBackup.ApiEngine;
    using PerforceBackup.Engine;
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Logger;
    using PerforceBackup.Engine.Models;
    using System;
    using System.Reflection;

    public class Program
    {
        public const string LogArhiveName = "Backup_logs";
        public const string CheckpointArhiveName = "Backup_ckp";
        public const string Log4NetPath = @"Logs\ResultLog.txt";

        public const string uri = "localhost:1666";
        public const string user = "Stefan.Mihaylov";
        public const string ws_client = "Hadess_2528";

        private static ILog logger;

        public static void Main()
        {
            try
            {
                logger = LogManager.GetLogger("PerforceBackup");
                Logger.Setup(Log4NetPath);

                var commands = new PerforceCommands(uri, user, ws_client);

                commands.StartServer();
              //  var verify = commands.Verify();

                var counters = commands.GetCounters();
                var userCount = commands.GetUsersCount();
                var sizes = commands.GetSizes();
                var serverInfo = commands.GetServerVersion();
                var projectCount = commands.GetProjectCount();
              //  var stop = commands.StopServer();

                return;

                var configurations = new Configurations();
                IEngineManager engineManager = new EngineManager(logger, configurations);
                var checkpointLogInfo = new CheckPointLogModel();

                Console.WriteLine(" - Start...");
                logger.Info("PerforceBackup started...");

                checkpointLogInfo.StartDate = DateTime.Now;
                Console.WriteLine(" - Date: {0}", checkpointLogInfo.StartDate.ToString(StringConstrants.DateTimeFormat));

                // Start Server if necessery
                var cmd = engineManager.CommandPromptExecutor;
                Console.WriteLine(" - Start Server: {0}", cmd.StartService("Perforce", "successfully") ? "Done" : "NO"); // */

                // validate
                var executor = engineManager.PerforceCommandLineExecutor;
                Console.WriteLine(" - Validate: {0}", executor.ValidateCommand() ? "OK" : "NO");

                // Checkpoint
                var server = engineManager.PerforceServerExecutor;
                // /*
                Console.WriteLine(" - Stop Server: {0}", executor.StopServerCommand() ? "Done" : "NO");
                Console.WriteLine(" - Checkpoint: {0}", server.MakeCheckPoint(configurations.CheckpointSubPath) ? "Done" : "NO");
                // */

                // Backuped Lod and Auditlig Filed
                var logArhivator = engineManager.LogFileArhivator;
                checkpointLogInfo.Log = logArhivator.Compress("log", configurations.MaxLogSize, configurations.LogArhivePath, LogArhiveName);
                checkpointLogInfo.AuditLog = logArhivator.Compress("auditlog", configurations.MaxAuditLogSize, configurations.LogArhivePath, LogArhiveName);
                Console.WriteLine(" - Log: {0}", checkpointLogInfo.Log);
                Console.WriteLine(" - AuditLog: {0}", checkpointLogInfo.AuditLog);

                // /*
                // Remove old Checkpoints & make Arhive
                var checkpointBackup = engineManager.CheckpointArhivator;
                Console.Write(" - Checkpoints Arhive: ");
                var checkpoint = checkpointBackup.Compress(configurations.LogArhivePath, CheckpointArhiveName);
                Console.WriteLine("{0}", checkpoint.IsOldCheckpointsCompressed ? "Done" : "NO");

                var rootArhivator = engineManager.RootArhivator;
                Console.Write(" - Root Arhive: ");
                checkpointLogInfo.Arhive = rootArhivator.Compress(checkpoint, configurations);
                Console.WriteLine("{0}", checkpointLogInfo.Arhive);

                // Start Server & end Backup; Statistics
                Console.WriteLine(" - Start Server: {0}", cmd.StartService("Perforce", "successfully") ? "Done" : "NO"); // */

                checkpointLogInfo.Counters = executor.CountersCommand();
                Console.WriteLine(" - Counters: {0}", checkpointLogInfo.Counters);

                checkpointLogInfo.Sizes = executor.SizeCommand();
                Console.WriteLine(" - Sizes: {0}", checkpointLogInfo.Sizes);

                var dir = engineManager.DirectoryInformation;
                checkpointLogInfo.DepotSize = dir.DirSizeInMb(configurations.DepotPath);
                Console.WriteLine(" - Depot Size: {0:0.00}Mb ", checkpointLogInfo.DepotSize);

                checkpointLogInfo.Users = executor.UsersCountCommand();
                Console.WriteLine(" - Users: {0}", checkpointLogInfo.Users);

                checkpointLogInfo.ServerInfo = server.GetServerVersion();
                Console.WriteLine(" - Server: {0}", checkpointLogInfo.ServerInfo);

                // Write to Excel
                var excelWriter = engineManager.ExcelWriter;
                excelWriter.AddRow(checkpointLogInfo);

                rootArhivator.AddFileToArhiv(excelWriter.FullFileRoot, checkpointLogInfo.Arhive.ArhiveFullPath);

                logger.Info("Data saved to Excel");

                excelWriter.OpenExcel();

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
