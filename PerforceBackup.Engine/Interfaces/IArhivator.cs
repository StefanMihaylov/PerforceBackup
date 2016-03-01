namespace PerforceBackup.Engine.Interfaces
{
    public interface IArhivator : IService
    {
        //void Arhive(string sourceName, string arhivePath, string arhiveName);

        //void Arhive(string sourceName, string arhivePath, string arhiveName, string arhiveType);

        void Arhive(string sourceName, string arhiveFullPath);
    }
}
