namespace PerforceBackup.Engine.Interfaces
{
    using PerforceBackup.Engine.Models;

    public interface IPerforceServerExecutor : IService
    {
        ServerVersionModel GetServerVersion();

        bool MakeCheckPoint(string checkpointSubPath);
    }
}
