namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Base;
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using System;
    using System.Globalization;
    using System.IO;

    public class LogFileArhivator : BaseArhivator, ILogFileArhivator
    {
        public const double DefaultMaxSize = 1.0;

        private string logFullPath;
        private IInfoLogger logger;

        public LogFileArhivator(IArhivator arhivator, IInfoLogger logger, string rootPath, string logFileSubPath)
            : base(arhivator)
        {
            this.logFullPath = Path.Combine(rootPath, logFileSubPath);
            this.logger = logger;
        }

        public LogFileModel Compress(string logFileName, string maxSize, string arhivePath, string arhiveName)
        {
            this.logger.Write(" - {0}{1}: ", char.ToUpper(logFileName[0]), logFileName.Substring(1));

            var result = new LogFileModel()
            {
                LogFileName = logFileName,
                IsCompress = false
            };

            var logFileFullPath = Path.Combine(this.logFullPath, logFileName);
            var isExist = File.Exists(logFileFullPath);
            result.IsExist = isExist;

            if (isExist)
            {
                var logFile = new FileInfo(logFileFullPath);
                var logFileSize = logFile.Length / 1024.0 / 1024;
                result.FileSize = logFileSize;

                if (logFileSize > this.GetMaxSize(maxSize))
                {
                    var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
                    var newFileName = string.Format("{0} {1}.txt", logFileName, currentDate);
                    var renamedFileFullPath = Path.Combine(this.logFullPath, newFileName);
                    File.Move(logFileFullPath, renamedFileFullPath);

                    var arhiveFullPath = this.Arhivator.Arhive(renamedFileFullPath, arhivePath, arhiveName);
                    if (!string.IsNullOrWhiteSpace(arhiveFullPath))
                    {
                        result.IsCompress = true;
                        File.SetAttributes(renamedFileFullPath, FileAttributes.Normal);
                        File.Delete(renamedFileFullPath);
                    }
                }
            }

            this.logger.WriteLine("{0}", result);
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
