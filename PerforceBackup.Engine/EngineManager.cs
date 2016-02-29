namespace PerforceBackup.Engine
{
    using log4net;

    using PerforceBackup.Engine.Arhivators;
    using PerforceBackup.Engine.Excel;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Services;

    using System;
    using System.Collections.Generic;
    using PerforceBackup.Engine.ApiCommands;

    public class EngineManager : IEngineManager
    {
        protected readonly IDictionary<Type, object> services;

        public EngineManager(ILog logger, IInfoLogger infoLogger, IConfigurations configurations)
        {
            this.Logger = logger;
            this.InfoLogger = infoLogger;
            this.Configurations = configurations;

            this.services = new Dictionary<Type, object>();
        }

        public ILog Logger { get; private set; }

        public IInfoLogger InfoLogger { get; private set; }

        public IConfigurations Configurations { get; private set; }

        public IPerforceCommands PerforceCommands
        {
            get
            {
                return (IPerforceCommands)this.GetRepository<PerforceCommands>();
            }
        }

        public ICommandPromptExecutor CommandPromptExecutor
        {
            get
            {
                return (ICommandPromptExecutor)this.GetRepository<CommandPromptExecutor>();
            }
        }

        public IPerforceCommandLineExecutor PerforceCommandLineExecutor
        {
            get
            {
                return (IPerforceCommandLineExecutor)this.GetRepository<PerforceCommandLineExecutor>();
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
                return (IArhivator)this.GetRepository<SevenZipSharp>();
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
                else if (typeof(T).IsAssignableFrom(typeof(CommandPromptExecutor)))
                {
                    var parameters = new object[] { this.Logger, this.InfoLogger };
                    instance = Activator.CreateInstance(typeof(CommandPromptExecutor), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(PerforceCommandLineExecutor)))
                {
                    var parameters = new object[] { this.Logger, this.Configurations.RootPath, string.Empty };
                    instance = Activator.CreateInstance(typeof(PerforceCommandLineExecutor), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(PerforceServerExecutor)))
                {
                    var parameters = new object[] { this.Logger, this.InfoLogger, this.Configurations.RootPath, this.Configurations.ServerSubPath };
                    instance = Activator.CreateInstance(typeof(PerforceServerExecutor), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(SevenZip_old)))
                {
                    var parameters = new object[] { this.Logger, this.Configurations.RootPath, this.Configurations.SevenZipSubPath };
                    instance = Activator.CreateInstance(typeof(SevenZip_old), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(SevenZipSharp)))
                {
                    //var parameters = new object[] { this.Logger, this.Configurations.RootPath, this.Configurations.SevenZipSubPath };
                    instance = Activator.CreateInstance(typeof(SevenZipSharp));
                }
                else if (typeof(T).IsAssignableFrom(typeof(LogFileArhivator)))
                {
                    var parameters = new object[] { this.SevenZip, this.InfoLogger, this.Configurations.RootPath, this.Configurations.LogFilesSubPath };
                    instance = Activator.CreateInstance(typeof(LogFileArhivator), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(CheckpointArhivator)))
                {
                    var parameters = new object[] { this.SevenZip, this.InfoLogger, this.Configurations.RootPath, this.Configurations.JournalSubPath };
                    instance = Activator.CreateInstance(typeof(CheckpointArhivator), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(RootArhivator)))
                {
                    var parameters = new object[] { this.SevenZip, this.InfoLogger, this.Configurations.RootPath };
                    instance = Activator.CreateInstance(typeof(RootArhivator), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(DirectoryInformation)))
                {
                    var parameters = new object[] { };
                    instance = Activator.CreateInstance(typeof(DirectoryInformation), parameters);
                }
                else if (typeof(T).IsAssignableFrom(typeof(ExcelWriter)))
                {
                    var parameters = new object[] { this.Configurations.BackupArhivePath, this.Configurations.CheckpointLogName, this.Configurations.CheckpointLogExtension };
                    instance = Activator.CreateInstance(typeof(ExcelWriter), parameters);
                }

                this.services.Add(typeof(T), instance);
            }

            return (IService)this.services[typeof(T)];
        }
    }
}
