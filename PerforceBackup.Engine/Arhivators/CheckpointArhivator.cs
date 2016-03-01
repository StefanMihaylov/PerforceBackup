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

            var checkpointDir = new DirectoryInfo(this.CombinedPath);
            var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
            var todayCheckpointFilter = string.Format("{0}.ckp.", currentDate);

            var todayCheckpoints = checkpointDir.GetFiles()
                                                .Where(f => f.Name.Contains(todayCheckpointFilter) && !f.Name.Contains(".md5"))
                                                .Select(f => f.Name)
                                                .OrderBy(f => f, new AlphanumComparator())
                                                .ToList();

            if (todayCheckpoints.Count == 0)
            {
                this.Logger.WriteLine(StringConstrants.FailMessage);
                return null;
            }

            string currentCheckpointName = todayCheckpoints.Last();

            IList<string> filesForArhive = this.GetFilesForArhive(checkpointDir, currentDate);
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

        private List<string> GetFilesForArhive(DirectoryInfo checkpointDir, string currentDate)
        {
            var oldCheckPoint = checkpointDir.GetFiles()
                                           .Where(f => f.Name.Contains(".ckp.") && !f.Name.Contains(currentDate))
                                           .Select(f => f.Name)
                                           .ToList();

            var oldCheckpointsDate = new HashSet<string>();
            foreach (var name in oldCheckPoint)
            {
                var nameSubParts = name.Split(new string[] { ".ckp." }, StringSplitOptions.RemoveEmptyEntries);
                oldCheckpointsDate.Add(nameSubParts[0]);
            }

            var filesForArhive = new List<string>();
            foreach (var date in oldCheckpointsDate)
            {
                filesForArhive.AddRange(checkpointDir.GetFiles()
                                                     .Where(f => f.Name.Contains(date))
                                                     .Select(f => f.FullName));
            }

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
