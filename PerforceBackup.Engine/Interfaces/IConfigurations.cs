namespace PerforceBackup.Engine.Interfaces
{
    public interface IConfigurations
    {
        string ServerUrl { get; }

        string ServerUser { get; }

        string UserWorkSpace { get; }


        string DepotSubPath { get; }

        string BackupSubPath { get; }

        string CheckpointSubPath { get; }

        string LogArhiveSubPath { get; }

        string CheckpointArhiveSubPath { get; }

        string CheckpointLogPath { get; }

        string MaxLogSize { get; }

        string MaxAuditLogSize { get; }

        string RootArhiveSubPath { get; }

        string ArhivePath { get; }
    }
}
