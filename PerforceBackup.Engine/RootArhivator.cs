namespace PerforceBackup.Engine
{
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using System.IO;

    public class RootArhivator : BaseArhivator 
    {
        private string rootPath;

        public RootArhivator(IArhivator arhivator, string rootPath)
            : base(arhivator)
        {
            this.rootPath = rootPath;
        }

        public ArhiveModel Compress(CheckpointModel checkpoint, string arhiveRoot, string arhiveName, string arhiveType)
        {
            var result = new ArhiveModel()
            {
                IsCompressed = false,
                Path = arhiveRoot,
                ArhivePatternName = string.Format("{0}*.{1}", arhiveName, arhiveType)
            };

            if (!checkpoint.IsCheckPointExist)
            {
                return result;
            }

            if (!Directory.Exists(arhiveRoot))
            {
                Directory.CreateDirectory(arhiveRoot);
            }

            var arhiveFullName = string.Format("{0} {1}", arhiveName, checkpoint.CheckpointName);
            result.ArhiveFullPath = this.Arhivator.Arhive(this.rootPath, arhiveRoot, arhiveFullName, arhiveType);
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
