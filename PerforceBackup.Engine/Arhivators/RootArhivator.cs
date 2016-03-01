namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using System.IO;
    using System;
    using PerforceBackup.Engine.Common;

    public class RootArhivator : BaseArhivator, IRootArhivator
    {
        public RootArhivator(IArhivator arhivator, IInfoLogger logger, string rootPath, string rootArhiveSubPath)
            : base(arhivator, logger, rootPath, rootArhiveSubPath)
        {
        }

        public ArhiveModel Compress(string checkpointName, string arhivePath)
        {
            this.Logger.Write(" - Root Arhive: ");

            FileInfo arhiveFileInfo = new FileInfo(arhivePath);
            var result = new ArhiveModel()
            {
                IsCompressed = false,
                Path = arhiveFileInfo.DirectoryName,
                ArhivePatternName = string.Format("{0}*.7z", arhiveFileInfo.Name)
            };

            if (string.IsNullOrWhiteSpace(checkpointName))
            {
                checkpointName = DateTime.Now.ToString(StringConstrants.DateFormat);
            }

            if (!Directory.Exists(result.Path))
            {
                Directory.CreateDirectory(result.Path);
            }

            var arhiveFullName = string.Format("{0} {1}", arhiveFileInfo.Name, checkpointName);
            result.ArhiveFullPath = Path.GetFullPath(Path.Combine(result.Path, string.Format("{0}.7z", arhiveFullName)));

            this.Arhivator.Arhive(this.CombinedPath, result.ArhiveFullPath);

            result.IsCompressed = true;
            var file = new FileInfo(result.ArhiveFullPath);
            var fileSize = file.Length / 1024.0 / 1024;
            result.Size = fileSize;

            this.Logger.WriteLine("{0}", result);
            return result;
        }

        public void AddFileToArhiv(string sourceFullPath, string arhiveFullPath)
        {
            this.Arhivator.Arhive(sourceFullPath, arhiveFullPath);
        }
    }
}
