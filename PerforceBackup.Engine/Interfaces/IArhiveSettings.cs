namespace PerforceBackup.Engine.Interfaces
{
    public interface IArhiveSettings
    {
        string ArhiveName { get; }

        string ArhivePath { get; }

        string ArhiveType { get; }
    }
}
