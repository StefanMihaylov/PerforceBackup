
namespace PerforceBackup.Engine.Interfaces
{
    public interface ICommandPromptExecutor : IService
    {
        bool StartService(string serviceName, string successString);
    }
}
