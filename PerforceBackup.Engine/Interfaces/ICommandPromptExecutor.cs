namespace PerforceBackup.Engine.Interfaces
{
    public interface ICommandPromptExecutor : IService
    {
        bool StartService(string serviceName = "Perforce");

        bool StopService(string serviceName = "Perforce");
    }
}
