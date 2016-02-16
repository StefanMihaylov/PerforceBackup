namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Base;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using System.IO;

    public class RootArhivator : BaseArhivator, IRootArhivator 
    {
        private string rootPath;

        public RootArhivator(IArhivator arhivator, string rootPath)
            : base(arhivator)
        {
            this.rootPath = rootPath;
        }

        public ArhiveModel Compress(CheckpointModel checkpoint, IArhiveSettings settings)
        {
            var result = new ArhiveModel()
            {
                IsCompressed = false,
                Path = settings.ArhivePath,
                ArhivePatternName = string.Format("{0}*.{1}", settings.ArhiveName, settings.ArhiveType)
            };

            if (!checkpoint.IsCheckPointExist)
            {
                return result;
            }

            if (!Directory.Exists(result.Path))
            {
                Directory.CreateDirectory(result.Path);
            }

            var arhiveFullName = string.Format("{0} {1}", settings.ArhiveName, checkpoint.CheckpointName);
            result.ArhiveFullPath = this.Arhivator.Arhive(this.rootPath, result.Path, arhiveFullName, settings.ArhiveType);
            result.IsCompressed = !string.IsNullOrWhiteSpace(result.ArhiveFullPath);

            if (result.IsCompressed)
            {
                var file = new FileInfo(result.ArhiveFullPath);
                var fileSize = file.Length / 1024.0 / 1024;
                result.Size = fileSize;
            }

            return result;
        }

        public void AddFileToArhiv(string sourceFullPath, string arhiveFullPath)
        {
            var result = this.Arhivator.Arhive(sourceFullPath, arhiveFullPath);
        }
    }
}
