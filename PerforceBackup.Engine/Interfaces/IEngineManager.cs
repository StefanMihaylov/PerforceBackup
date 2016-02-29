namespace PerforceBackup.Engine.Interfaces
{
    using log4net;

    public interface IEngineManager
    {
        ILog Logger { get; }

        IInfoLogger InfoLogger { get; }

        IConfigurations Configurations { get; }

        IPerforceCommands PerforceCommands { get; }

        ICheckpointArhivator CheckpointArhivator { get; }

        ICommandPromptExecutor CommandPromptExecutor { get; }

        IDirectoryInformation DirectoryInformation { get; }

        IExcelWriter ExcelWriter { get; }

        ILogFileArhivator LogFileArhivator { get; }

        IPerforceCommandLineExecutor PerforceCommandLineExecutor { get; }

        IPerforceServerExecutor PerforceServerExecutor { get; }

        IRootArhivator RootArhivator { get; }

        IArhivator SevenZip { get; }
    }
}
