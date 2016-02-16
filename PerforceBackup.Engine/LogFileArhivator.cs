﻿namespace PerforceBackup.Engine
{
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using System;
    using System.Globalization;
    using System.IO;

    public class LogFileArhivator : BaseArhivator
    {
        public const double DefaultMaxSize = 1.0;
        private string logFullPath;

        public LogFileArhivator(IArhivator arhivator, string rootPath, string logFileSubPath)
            : base(arhivator)
        {
            this.logFullPath = Path.Combine(rootPath, logFileSubPath);
        }

        public static double GetMaxSize(string size)
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

        public LogFileModel Compress(string logFileName, double maxSize, string arhivePath, string arhiveName)
        {
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

                if (logFileSize > maxSize)
                {
                    var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
                    var newFileName = string.Format("{0} {1}.txt", logFileName, currentDate);
                    var renamedFileFullPath = Path.Combine(this.logFullPath, newFileName);
                    File.Move(logFileFullPath, renamedFileFullPath);

                    var arhiveFullPath = this.Arhivator.Arhive(arhivePath, arhiveName, renamedFileFullPath);
                    if (!string.IsNullOrWhiteSpace(arhiveFullPath))
                    {
                        result.IsCompress = true;
                        File.SetAttributes(renamedFileFullPath, FileAttributes.Normal);
                        File.Delete(renamedFileFullPath);
                    }
                }
            }

            return result;
        }
    }
}
