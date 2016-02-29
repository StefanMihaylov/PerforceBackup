namespace PerforceBackup.Engine.Interfaces
{
    using PerforceBackup.Engine.Models;

    public interface IPerforceCommands : IService
    {
        CountersModel GetCounters();

        int GetProjectCount();

        ServerVersionModel GetServerVersion();

        SizesModel GetSizes();

        int GetUsersCount();

        bool StopServer();

        bool Verify();

        string GetServerRoot();
    }
}
