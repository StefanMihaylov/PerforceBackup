namespace PerforceBackup.Engine.Arhivators
{
    using log4net;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Services;
    using System;
    using System.IO;

    public class SevenZip_old : CommandPromptExecutor, IArhivator
    {
        public const string DefaultArhiveType = "7z";

        private string sevenZipFullPath;

        public SevenZip_old(ILog logger, IInfoLogger infoLogger, string rootPath, string sevenZipSubPath)
            : base(logger, infoLogger)
        {
            this.sevenZipFullPath = Path.Combine(rootPath, sevenZipSubPath, "7z.exe");
        }

        public string Arhive(string sourceName, string arhivePath, string arhiveName)
        {
            return this.Arhive(sourceName, arhivePath, arhiveName, DefaultArhiveType);
        }

        public string Arhive(string sourceName, string arhivePath, string arhiveName, string arhiveType)
        {
            var arhiveFullPath = Path.Combine(arhivePath, string.Format("{0}.{1}", arhiveName, arhiveType));
            return Arhive(sourceName, arhiveFullPath);
        }

        public string Arhive(string sourceName, string arhiveFullPath)
        {
            var arhiveType = new FileInfo(arhiveFullPath).Extension.Substring(1);
            var result = this.ExecuteCommand(string.Format("{0} a -t{3} \"{1}\" \"{2}\" > null", this.sevenZipFullPath, arhiveFullPath, sourceName, arhiveType));
            if (string.IsNullOrWhiteSpace(result))
            {
                return arhiveFullPath;
            }
            else
            {
                throw new OperationCanceledException(result);
            }
        }
    }
}
