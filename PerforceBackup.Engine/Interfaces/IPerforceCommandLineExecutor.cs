namespace PerforceBackup.Engine.Interfaces
{
    using PerforceBackup.Engine.Models;

    public interface IPerforceCommandLineExecutor : IService
    {
        CountersModel CountersCommand();

        SizesModel SizeCommand();

        bool StopServerCommand();

        int UsersCountCommand();

        bool ValidateCommand();
    }
}
