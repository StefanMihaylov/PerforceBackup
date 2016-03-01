namespace PerforceBackup.Engine.Arhivators
{
    using System;
    using System.IO;
    using System.Reflection;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Services;

    public class SevenZip : CommandExecutor, IArhivator
    {
        public const string DefaultArhiveType = "7z";

        public SevenZip(IResultLogger logger)
            : base(logger, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), string.Empty, "7z.exe")
        {
        }

        public void Arhive(string sourceName, string arhiveFullPath)
        {
            var arhiveType = new FileInfo(arhiveFullPath).Extension.Substring(1);
            string command = string.Format("a -t{2} \"{0}\" \"{1}\"", arhiveFullPath, sourceName, arhiveType);

            var result = this.ExecuteCommand(command);
            if (result.Contains("Everything is Ok"))
            {
                return;
            }
            else
            {
                throw new OperationCanceledException(result);
            }
        }
    }
}
