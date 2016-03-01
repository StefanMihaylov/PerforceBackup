namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class CheckpointArhivator : BaseArhivator, ICheckpointArhivator
    {
        public CheckpointArhivator(IArhivator arhivator, IInfoLogger logger, string rootPath, string checkpointSubPath)
            : base(arhivator, logger, rootPath, checkpointSubPath)
        {
        }

        public string Compress(string arhivePath)
        {
            this.Logger.Write(" - Checkpoints Arhive: ");

            string arhiveFullPath = Path.GetFullPath(Path.Combine(this.RootPath, arhivePath));

            string currentCheckpointName = this.GetLastCheckpointName();
            if (string.IsNullOrWhiteSpace(currentCheckpointName))
            {
                this.Logger.WriteLine(StringConstrants.FailMessage);
                return null;
            }

            IList<string> filesForArhive = this.GetFilesForArhive(currentCheckpointName);
            if (filesForArhive.Count == 0)
            {
                this.Logger.WriteLine(StringConstrants.FailMessage);
                return currentCheckpointName;
            }

            foreach (string file in filesForArhive)
            {
                this.Arhivator.Arhive(file, arhiveFullPath);

                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            this.Logger.WriteLine(StringConstrants.SuccessMessage);
            return currentCheckpointName;
        }

        private string GetLastCheckpointName()
        {
            DirectoryInfo checkpointDir = new DirectoryInfo(this.CombinedPath);

            var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
            var todayCheckpointFilter = string.Format("{0}.ckp.", currentDate);

            List<string> todayCheckpoints = checkpointDir.GetFiles()
                                .Where(f => f.Name.Contains(todayCheckpointFilter) && !f.Name.Contains(".md5"))
                                .Select(f => f.Name)
                                .OrderBy(f => f, new AlphanumComparator())
                                .ToList();

            if (todayCheckpoints.Count == 0)
            {
                return null;
            }

            return todayCheckpoints.Last();
        }

        private IList<string> GetFilesForArhive(string currentCheckpointName)
        {
            DirectoryInfo checkpointDir = new DirectoryInfo(this.CombinedPath);

            var nameSubParts = currentCheckpointName.Split(new string[] { ".ckp." }, StringSplitOptions.RemoveEmptyEntries);
            var checkpointNumber = int.Parse(nameSubParts[1]);

            var lastCheckpointFiles = new HashSet<string>();
            lastCheckpointFiles.Add(currentCheckpointName);
            lastCheckpointFiles.Add(string.Format("{0}.md5", currentCheckpointName));
            lastCheckpointFiles.Add(string.Format("{0}.jnl.{1}", nameSubParts[0], checkpointNumber - 1));

            IList<string> filesForArhive = checkpointDir.GetFiles()
                                                     .Where(f => f.Name.Contains(".ckp.") || f.Name.Contains(".jnl."))
                                                     .Where(f => !lastCheckpointFiles.Contains(f.Name))
                                                     .Select(f => f.FullName)
                                                     .ToList();
            return filesForArhive;
        }

        private int ExtractNumber(string text)
        {
            var match = Regex.Match(text, @"(\d+)");
            if (match == null)
            {
                return 0;
            }

            int value;
            if (!int.TryParse(match.Value, out value))
            {
                return 0;
            }

            return value;
        }
    }
}
