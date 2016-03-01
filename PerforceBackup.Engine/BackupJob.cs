namespace PerforceBackup.Engine
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
        private string version;

        public BackupJob(IEngineManager engineManager, string version = null)
        {
            this.EngineManager = engineManager;
            this.version = version;
        }

        public BackupJob(ILog logger, IInfoLogger infoLogger, IConfigurations configurations, string version = null)
            : this(new EngineManager(logger, infoLogger, configurations))
        {
            this.version = version;
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

        public void MakeArhive(CheckPointLogModel result)
        {
            IRootArhivator rootArhivator = this.EngineManager.RootArhivator;
            result.Arhive = rootArhivator.Compress(result.CheckpointName, this.Configurations.ArhivePath);
        }

        public void WriteToExcel(CheckPointLogModel result)
        {
            IExcelWriter excelWriter = this.EngineManager.ExcelWriter;
            excelWriter.AddRow(result);

            IRootArhivator rootArhivator = this.EngineManager.RootArhivator;
            rootArhivator.AddFileToArhiv(excelWriter.FullFileRoot, result.Arhive.ArhiveFullPath);
            this.Logger.Info("Data saved to Excel");
        }

        public void MakeCheckpoint(CheckPointLogModel result)
        {
            IPerforceServerExecutor server = this.EngineManager.PerforceServerExecutor;
            server.MakeCheckPoint(this.Configurations.CheckpointSubPath);

            // Remove old Checkpoints
            ICheckpointArhivator checkpointBackup = this.EngineManager.CheckpointArhivator;
            result.CheckpointName = checkpointBackup.Compress(this.Configurations.CheckpointArhiveSubPath);
        }

        public void BackupLogs(CheckPointLogModel result)
        {
            ILogFileArhivator logArhivator = this.EngineManager.LogFileArhivator;
            result.Log = logArhivator.Compress("log", this.Configurations.MaxLogSize, this.Configurations.LogArhiveSubPath);
            result.AuditLog = logArhivator.Compress("auditlog", this.Configurations.MaxAuditLogSize, this.Configurations.LogArhiveSubPath);
        }

        public void Initialize(CheckPointLogModel result)
        {
            this.InfoLogger.WriteLine(" - Start...");
            string version = string.Empty;
            if (string.IsNullOrWhiteSpace(this.version))
            {
                version = string.Format("v.{0}", this.version);
            }

            this.Logger.InfoFormat("PerforceBackup {0} started...", version);
            this.InfoLogger.WriteLine(" - Date: {0}", result.StartDate.ToString(StringConstrants.DateTimeFormat));

            this.StartService();
        }

        public void EndMessage()
        {
            this.Logger.Info("End" + Environment.NewLine);
            this.InfoLogger.WriteLine(" - End");
        }

        public void Validate()
        {
            IPerforceCommands perforce = this.EngineManager.PerforceCommands;
            perforce.Verify();
        }

        public void GetStatistics(CheckPointLogModel result)
        {
            IPerforceCommands perforce = this.EngineManager.PerforceCommands;

            result.ServerInfo = perforce.GetServerVersion();
            result.Counters = perforce.GetCounters();
            result.Sizes = perforce.GetSizes();
            result.UserCount = perforce.GetUsersCount();
            result.ProjectCount = perforce.GetProjectCount();

            IDirectoryInformation dirInfo = this.EngineManager.DirectoryInformation;
            result.DepotSize = dirInfo.DirSizeInMb(perforce.ServerRoot, this.Configurations.DepotSubPath, this.InfoLogger);
        }

        public void StopService()
        {
            IPerforceServerExecutor server = this.EngineManager.PerforceServerExecutor;
            server.StopService();
        }

        public void StartService()
        {
            IPerforceServerExecutor server = this.EngineManager.PerforceServerExecutor;
            server.StartService();
        }
    }
}
