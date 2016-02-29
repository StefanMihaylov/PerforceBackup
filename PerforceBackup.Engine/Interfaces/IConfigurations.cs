namespace PerforceBackup.Engine.Interfaces
{
    public interface IConfigurations
    {
        string ServerUrl { get; }

        string DepotSubPath { get; }



        string RootPath { get; }

        string ServerSubPath { get; }        

        string JournalSubPath { get; }

        string CheckpointSubPath { get; }

        string BackupArhiveSubPath { get; }

        string SevenZipSubPath { get; }

        string LogFilesSubPath { get; }

        string MaxLogSize { get; }

        string MaxAuditLogSize { get; }                

        string CheckpointLogName { get; }

        string CheckpointLogExtension { get; }


        // combined paths 
        string LogArhivePath { get; }

        string BackupArhivePath { get; }
    }
}
