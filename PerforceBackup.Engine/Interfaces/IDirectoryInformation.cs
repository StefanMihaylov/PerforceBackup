namespace PerforceBackup.Engine.Interfaces
{
    using System.IO;

    public interface IDirectoryInformation : IService
    {
        long DirSize(DirectoryInfo directory);

        long DirSize(string rootPath, string depotSubPath);

        long DirSize(string fullPath);

        double DirSizeInMb(string rootPath, string depotSubPath);

        double DirSizeInMb(string fullPath);
    }
}
