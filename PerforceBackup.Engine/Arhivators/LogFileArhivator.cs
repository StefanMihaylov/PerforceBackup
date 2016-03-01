namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using System;
    using System.Globalization;
    using System.IO;

    public class LogFileArhivator : BaseArhivator, ILogFileArhivator
    {
        public const double DefaultMaxSize = 1.0;

        public LogFileArhivator(IArhivator arhivator, IInfoLogger logger, string rootPath, string logFileSubPath)
            : base(arhivator, logger, rootPath, logFileSubPath)
        {
        }

        public LogFileModel Compress(string logFileName, string maxSize, string arhivePath)
        {
            this.Logger.Write(" - {0}{1}: ", char.ToUpper(logFileName[0]), logFileName.Substring(1));

            var result = new LogFileModel()
            {
                LogFileName = logFileName,
                IsCompress = false
            };

            var logFileFullPath = Path.Combine(this.CombinedPath, logFileName);
            var arhiveFullPath = Path.GetFullPath(Path.Combine(this.RootPath, arhivePath));

            result.IsExist = File.Exists(logFileFullPath);
            if (result.IsExist)
            {
                var logFile = new FileInfo(logFileFullPath);
                var logFileSize = logFile.Length / 1024.0 / 1024;
                result.FileSize = logFileSize;

                if (logFileSize >= this.GetMaxSize(maxSize))
                {
                    var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
                    var newFileName = string.Format("{0} {1}.txt", logFileName, currentDate);
                    var renamedFileFullPath = Path.Combine(this.CombinedPath, newFileName);
                    File.Move(logFileFullPath, renamedFileFullPath);

                    this.Arhivator.Arhive(renamedFileFullPath, arhiveFullPath);

                    result.IsCompress = true;
                    File.SetAttributes(renamedFileFullPath, FileAttributes.Normal);
                    File.Delete(renamedFileFullPath);
                }
            }

            this.Logger.WriteLine("{0}", result);
            return result;
        }

        private double GetMaxSize(string size)
        {
            double maxSize;
            if (double.TryParse(size, out maxSize))
            {
                return maxSize;
            }
            else
            {
                return DefaultMaxSize;
            }
        }
    }
}
