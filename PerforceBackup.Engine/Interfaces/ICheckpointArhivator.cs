namespace PerforceBackup.Engine.Interfaces
{
    using PerforceBackup.Engine.Models;

    public interface ICheckpointArhivator : IService
    {
        CheckpointModel Compress(string arhivePath, string arhiveName);
    }
}
