namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Base;
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
        private string CheckpointFullPath;

        public CheckpointArhivator(IArhivator arhivator, string rootPath, string checkpointSubPath)
            : base(arhivator)
        {
            this.CheckpointFullPath = Path.Combine(rootPath, checkpointSubPath);
        }

        public CheckpointModel Compress(string arhivePath, string arhiveName)
        {
            var result = new CheckpointModel()
            {
                IsCheckPointExist = false,
                IsOldCheckpointsCompressed = false
            };

            var checkpointDir = new DirectoryInfo(this.CheckpointFullPath);
            var currentDate = DateTime.Now.ToString(StringConstrants.DateFormat);
            var todayCheckpointFilter = string.Format("{0}.ckp.", currentDate);

            var todayCheckpoints = checkpointDir.GetFiles()
                                                .Where(f => f.Name.Contains(todayCheckpointFilter) && !f.Name.Contains(".md5"))
                                                .Select(f => f.Name)
                                                .OrderBy(f => f, new AlphanumComparator())
                                                .ToList();

            if (todayCheckpoints.Count == 0)
            {
                return result;
            }
            else
            {
                result.IsCheckPointExist = true;
                result.CheckpointName = todayCheckpoints.Last();

            }

            var filesForArhive = this.GetFilesForArhive(checkpointDir, currentDate);
            if (filesForArhive.Count == 0)
            {
                return result;
            }

            foreach (var file in filesForArhive)
            {
                var arhiveFullPath = this.Arhivator.Arhive(file, arhivePath, arhiveName);
                if (!string.IsNullOrWhiteSpace(arhiveFullPath))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                else
                {
                    return result;
                }
            }

            result.IsOldCheckpointsCompressed = true;
            return result;
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

        // Note: This could very well be a bad implementation. I'm not too great with Regex.
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
