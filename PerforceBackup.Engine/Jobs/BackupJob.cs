namespace PerforceBackup.Engine.Jobs
{
    using System;
    using System.IO;
    using System.Threading;
    using log4net;
    using PerforceBackup.Engine.ApiCommands;
    using PerforceBackup.Engine.Arhivators;
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using PerforceBackup.Engine.Services;

    public class BackupJob : IJob
    {
        public const string LogArhiveName = "Backup_logs";
        public const string CheckpointArhiveName = "Backup_ckp";

        public BackupJob(IEngineManager engineManager)
        {
            this.EngineManager = engineManager;
        }

        public BackupJob(ILog logger, IInfoLogger infoLogger, IConfigurations configurations)
            : this(new EngineManager(logger, infoLogger, configurations))
        {
        }

        protected IEngineManager EngineManager { get; private set; }

        protected ILog Logger
        {
            get
            {
                return this.EngineManager.Logger;
            }
        }

        protected IInfoLogger InfoLogger
        {
            get
            {
                return this.EngineManager.InfoLogger;
            }
        }

        protected IConfigurations Configurations
        {
            get
            {
                return this.EngineManager.Configurations;
            }
        }

        public void Execute()
        {
            CheckPointLogModel result = new CheckPointLogModel();

            this.Initialize(result);

            this.Validate();

            this.StopService();

            this.MakeCheckpoint(result);

            this.BackupLogs(result);

            this.MakeArhive(result);

            this.StartService();

            this.GetStatistics(result);

            this.WriteToExcel(result);

            this.EndMessage();

            this.EngineManager.ExcelWriter.OpenExcel();
        }

        private void MakeArhive(CheckPointLogModel result)
        {
            IRootArhivator rootArhivator = this.EngineManager.RootArhivator;
            result.Arhive = rootArhivator.Compress(result.CheckpointName, (IArhiveSettings)this.Configurations);
        }

        protected void WriteToExcel(CheckPointLogModel result)
        {
            IExcelWriter excelWriter = this.EngineManager.ExcelWriter;
            excelWriter.AddRow(result);

            IRootArhivator rootArhivator = this.EngineManager.RootArhivator;
            rootArhivator.AddFileToArhiv(excelWriter.FullFileRoot, result.Arhive.ArhiveFullPath);
            this.Logger.Info("Data saved to Excel");
        }

        protected void MakeCheckpoint(CheckPointLogModel result)
        {
            IPerforceServerExecutor server = this.EngineManager.PerforceServerExecutor;
            server.MakeCheckPoint(this.Configurations.CheckpointSubPath);

            // Remove old Checkpoints
            ICheckpointArhivator checkpointBackup = this.EngineManager.CheckpointArhivator;
            result.CheckpointName = checkpointBackup.Compress(this.Configurations.LogArhivePath, CheckpointArhiveName);
        }

        protected void BackupLogs(CheckPointLogModel result)
        {
            ILogFileArhivator logArhivator = this.EngineManager.LogFileArhivator;
            result.Log = logArhivator.Compress("log", this.Configurations.MaxLogSize, this.Configurations.LogArhivePath, LogArhiveName);
            result.AuditLog = logArhivator.Compress("auditlog", this.Configurations.MaxAuditLogSize, this.Configurations.LogArhivePath, LogArhiveName);
        }

        protected void Initialize(CheckPointLogModel result)
        {
            this.InfoLogger.WriteLine(" - Start...");
            this.Logger.Info("PerforceBackup started...");
            this.InfoLogger.WriteLine(" - Date: {0}", result.StartDate.ToString(StringConstrants.DateTimeFormat));

            this.StartService();
        }

        protected void EndMessage()
        {
            this.Logger.Info("End" + Environment.NewLine);
            this.InfoLogger.WriteLine(" - End");
        }

        protected void Validate()
        {
            IPerforceCommands perforce = this.EngineManager.PerforceCommands;
            perforce.Verify();
        }

        protected void GetStatistics(CheckPointLogModel result)
        {
            IPerforceCommands perforce = this.EngineManager.PerforceCommands;

            result.ServerInfo = perforce.GetServerVersion();
            result.Counters = perforce.GetCounters();
            result.Sizes = perforce.GetSizes();
            result.UserCount = perforce.GetUsersCount();
            result.ProjectCount = perforce.GetProjectCount();

            IDirectoryInformation dirInfo = this.EngineManager.DirectoryInformation;
            result.DepotSize = dirInfo.DirSizeInMb(perforce.GetServerRoot(), this.Configurations.DepotSubPath, this.InfoLogger);
        }

        protected void StopService()
        {
            ICommandPromptExecutor cmd = this.EngineManager.CommandPromptExecutor;
            cmd.StopService();
        }

        protected void StartService()
        {
            ICommandPromptExecutor cmd = this.EngineManager.CommandPromptExecutor;
            cmd.StartService();
        }
    }
}
