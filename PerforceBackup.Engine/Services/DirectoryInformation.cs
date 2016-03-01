namespace PerforceBackup.Engine.Services
{
    using PerforceBackup.Engine.Interfaces;
    using System.IO;

    public class DirectoryInformation : IDirectoryInformation
    {
        private const double ByteToMbRatio = 1024.0 * 1024;

        public double DirSizeInMb(string rootpath, string subPath, IInfoLogger logger = null)
        {
            var path = Path.GetFullPath(Path.Combine(rootpath, subPath));
            return this.DirSizeInMb(path, logger);
        }

        public double DirSizeInMb(string path, IInfoLogger logger = null)
        {
            if (logger != null)
            {
                logger.Write(" - Depot Size: ");
            }

            DirectoryInfo directory = new DirectoryInfo(path);
            double result = this.DirSize(directory) / ByteToMbRatio;

            if (logger != null)
            {
                logger.WriteLine("{0:0.00}Mb", result);
            }

            return result;
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
