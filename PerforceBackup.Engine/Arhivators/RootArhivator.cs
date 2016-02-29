namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Base;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using System.IO;
    using System;
    using PerforceBackup.Engine.Common;

    public class RootArhivator : BaseArhivator, IRootArhivator
    {
        private string rootPath;
        private IInfoLogger logger;

        public RootArhivator(IArhivator arhivator, IInfoLogger logger, string rootPath)
            : base(arhivator)
        {
            this.rootPath = rootPath;
            this.logger = logger;
        }

        public ArhiveModel Compress(string checkpointName, IArhiveSettings settings)
        {
            this.logger.Write(" - Root Arhive: ");
            var result = new ArhiveModel()
            {
                IsCompressed = false,
                Path = settings.ArhivePath,
                ArhivePatternName = string.Format("{0}*.{1}", settings.ArhiveName, settings.ArhiveType)
            };

            if (string.IsNullOrWhiteSpace(checkpointName))
            {
                checkpointName = DateTime.Now.ToString(StringConstrants.DateFormat);
            }

            if (!Directory.Exists(result.Path))
            {
                Directory.CreateDirectory(result.Path);
            }

            var arhiveFullName = string.Format("{0} {1}", settings.ArhiveName, checkpointName);
            result.ArhiveFullPath = this.Arhivator.Arhive(this.rootPath, result.Path, arhiveFullName, settings.ArhiveType);
            result.IsCompressed = !string.IsNullOrWhiteSpace(result.ArhiveFullPath);

            if (result.IsCompressed)
            {
                var file = new FileInfo(result.ArhiveFullPath);
                var fileSize = file.Length / 1024.0 / 1024;
                result.Size = fileSize;
            }

            this.logger.WriteLine("{0}", result);
            return result;
        }

        public void AddFileToArhiv(string sourceFullPath, string arhiveFullPath)
        {
            var result = this.Arhivator.Arhive(sourceFullPath, arhiveFullPath);
        }
    }
}
