namespace PerforceBackup.Engine.Interfaces
{
    public interface ICheckpointArhivator : IService
    {
        string Compress(string arhivePath, string arhiveName);
    }
}
