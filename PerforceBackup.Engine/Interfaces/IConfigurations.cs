namespace PerforceBackup.Engine.Interfaces
{
    public interface IConfigurations
    {
        string RootPath { get; }

        string ServerSubPath { get; }

        string DepotSubPath { get; }

        string JournalSubPath { get; }

        string CheckpointSubPath { get; }

        string BackupArhiveSubPath { get; }

        string SevenZipSubPath { get; }

        string LogFilesSubPath { get; }

        string MaxLogSize { get; }

        string MaxAuditLogSize { get; }                

        string CheckpointLogName { get; }

        string CheckpointLogExtension { get; }

        // conmined paths 
        string LogArhivePath { get; }

        string DepotPath { get; }

        string BackupArhivePath { get; }
    }
}
