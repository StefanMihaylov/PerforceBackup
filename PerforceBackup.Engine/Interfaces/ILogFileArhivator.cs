namespace PerforceBackup.Engine.Interfaces
{
    using PerforceBackup.Engine.Models;

    public interface ILogFileArhivator : IService
    {
        LogFileModel Compress(string logFileName, string maxSize, string arhivePath, string arhiveName);
    }
}
