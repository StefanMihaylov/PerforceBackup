namespace PerforceBackup.Engine.Interfaces
{
    using System.IO;

    public interface IDirectoryInformation : IService
    {
        double DirSizeInMb(string rootpath, string subPath, IInfoLogger logger = null);

        double DirSizeInMb(string path, IInfoLogger logger = null);

        long DirSize(DirectoryInfo directory);
    }
}
