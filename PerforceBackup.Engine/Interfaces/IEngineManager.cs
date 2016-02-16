namespace PerforceBackup.Engine.Interfaces
{
    public interface IEngineManager
    {
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
