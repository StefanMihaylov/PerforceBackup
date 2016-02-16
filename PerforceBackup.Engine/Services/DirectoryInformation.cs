namespace PerforceBackup.Engine.Services
{
    using PerforceBackup.Engine.Interfaces;
    using System.IO;

    public class DirectoryInformation : IDirectoryInformation
    {
        private const double ByteToMbRatio = 1024.0 * 1024;

        public double DirSizeInMb(string rootPath, string depotSubPath)
        {
            return this.DirSize(rootPath, depotSubPath) / ByteToMbRatio;
        }

        public double DirSizeInMb(string fullPath)
        {
            return this.DirSize(fullPath) / ByteToMbRatio;
        }

        public long DirSize(string fullPath)
        {
            var directory = new DirectoryInfo(fullPath);
            return DirSize(directory);
        }


        public long DirSize(string rootPath, string depotSubPath)
        {
            var path = Path.Combine(rootPath, depotSubPath);
            return this.DirSize(path);
        }

        public long DirSize(DirectoryInfo directory)
        {
            long totalSize = 0;
            // Add file sizes.
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (DirectoryInfo currentDirectory in directories)
            {
                totalSize += DirSize(currentDirectory);
            }
            return totalSize;
        }
    }
}
