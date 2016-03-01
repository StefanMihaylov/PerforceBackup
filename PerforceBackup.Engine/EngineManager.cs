namespace PerforceBackup.Engine
{
    using System;
    using System.Collections.Generic;

    using PerforceBackup.Engine.ApiCommands;
    using PerforceBackup.Engine.Arhivators;
    using PerforceBackup.Engine.Excel;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Logger;
    using PerforceBackup.Engine.Services;

    public class EngineManager : IEngineManager
    {
        protected readonly IDictionary<Type, object> services;

        public EngineManager(IResultLogger logger, IInfoLogger infoLogger, IConfigurations configurations)
        {
            this.Logger = logger;
            this.InfoLogger = infoLogger;
            this.Configurations = configurations;

            this.services = new Dictionary<Type, object>();
        }

        public IResultLogger Logger { get; private set; }

        public IInfoLogger InfoLogger { get; private set; }

        public IConfigurations Configurations { get; private set; }

        public IPerforceCommands PerforceCommands
        {
            get
            {
                return (IPerforceCommands)this.GetRepository<PerforceCommands>();
            }
        }

        public IPerforceServerExecutor PerforceServerExecutor
        {
            get
            {
                return (IPerforceServerExecutor)this.GetRepository<PerforceServerExecutor>();
            }
        }

        public IArhivator SevenZip
        {
            get
            {
                return (IArhivator)this.GetRepository<SevenZip>();
            }
        }

        public ILogFileArhivator LogFileArhivator
        {
            get
            {
                return (ILogFileArhivator)this.GetRepository<LogFileArhivator>();
            }
        }

        public ICheckpointArhivator CheckpointArhivator
        {
            get
            {
                return (ICheckpointArhivator)this.GetRepository<CheckpointArhivator>();
            }
        }

        public IRootArhivator RootArhivator
        {
            get
            {
                return (IRootArhivator)this.GetRepository<RootArhivator>();
            }
        }

        public IDirectoryInformation DirectoryInformation
        {
            get
            {
                return (IDirectoryInformation)this.GetRepository<DirectoryInformation>();
            }
        }

        public IExcelWriter ExcelWriter
        {
            get
            {
                return (IExcelWriter)this.GetRepository<ExcelWriter>();
            }
        }

        protected virtual IService GetRepository<T>() where T : class
        {
            if (!this.services.ContainsKey(typeof(T)))
            {
                object instance = null;

                if (typeof(T).IsAssignableFrom(typeof(PerforceCommands)))
                {
                    var parameters = new object[] { this.Configurations.ServerUrl, this.InfoLogger };
                    instance = Activator.CreateInstance(typeof(PerforceCommands), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(PerforceServerExecutor)))
                {
                    var parameters = new object[] { new EmptyResultLogger(), this.InfoLogger, this.PerforceCommands.ServerRoot, string.Empty };
                    instance = Activator.CreateInstance(typeof(PerforceServerExecutor), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(SevenZip)))
                {
                    var parameters = new object[] { new EmptyResultLogger()}; // this.Logger
                    instance = Activator.CreateInstance(typeof(SevenZip), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(LogFileArhivator)))
                {
                    var parameters = new object[] { this.SevenZip, this.InfoLogger, this.PerforceCommands.ServerRoot, this.Configurations.BackupSubPath };
                    instance = Activator.CreateInstance(typeof(LogFileArhivator), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(CheckpointArhivator)))
                {
                    var parameters = new object[] { this.SevenZip, this.InfoLogger, this.PerforceCommands.ServerRoot, this.Configurations.BackupSubPath };
                    instance = Activator.CreateInstance(typeof(CheckpointArhivator), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(RootArhivator)))
                {
                    var parameters = new object[] { this.SevenZip, this.InfoLogger, this.PerforceCommands.ServerRoot, this.Configurations.RootArhiveSubPath };
                    instance = Activator.CreateInstance(typeof(RootArhivator), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(DirectoryInformation)))
                {
                    var parameters = new object[] { };
                    instance = Activator.CreateInstance(typeof(DirectoryInformation), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(ExcelWriter)))
                {
                    var parameters = new object[] { this.PerforceCommands.ServerRoot, this.Configurations.CheckpointLogPath };
                    instance = Activator.CreateInstance(typeof(ExcelWriter), parameters);
                }

                this.services.Add(typeof(T), instance);
            }

            return (IService)this.services[typeof(T)];
        }
    }
}
