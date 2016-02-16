namespace PerforceBackup.Engine
{
    using System.IO;

    public class DirectoryInformation
    {
        public double DirSizeInMb(string rootPath, string depotSubPath)
        {
            return this.DirSize(rootPath, depotSubPath) / 1024.0 / 1024;
        }

        public long DirSize(string rootPath, string depotSubPath)
        {
            var path = Path.Combine(rootPath, depotSubPath);
            var directory = new DirectoryInfo(path);
            return DirSize(directory);
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
