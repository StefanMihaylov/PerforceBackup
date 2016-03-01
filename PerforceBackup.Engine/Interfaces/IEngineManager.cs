namespace PerforceBackup.Engine.Interfaces
{
    public interface IEngineManager
    {
        IResultLogger Logger { get; }

        IInfoLogger InfoLogger { get; }

        IConfigurations Configurations { get; }

        IPerforceCommands PerforceCommands { get; }

        ICheckpointArhivator CheckpointArhivator { get; }

        IDirectoryInformation DirectoryInformation { get; }

        IExcelWriter ExcelWriter { get; }

        ILogFileArhivator LogFileArhivator { get; }

        IPerforceServerExecutor PerforceServerExecutor { get; }

        IRootArhivator RootArhivator { get; }

        IArhivator SevenZip { get; }
    }
}
