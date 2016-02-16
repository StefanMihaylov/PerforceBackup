namespace PerforceBackup.Engine.Interfaces
{
    public interface IArhivator : IService
    {
        string Arhive(string sourceName, string arhivePath, string arhiveName);

        string Arhive(string sourceName, string arhivePath, string arhiveName, string arhiveType);

        string Arhive(string sourceName, string arhiveFullPath);
    }
}
