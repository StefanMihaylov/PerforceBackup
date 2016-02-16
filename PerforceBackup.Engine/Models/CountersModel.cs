namespace PerforceBackup.Engine.Models
{
    using System;

    public class CountersModel
    {
        public long Change { get; set; }

        public long Journal { get; set; }

        public long Upgrade { get; set; }

        public long MaxCommitChange { get; set; }

        public override string ToString()
        {
            return string.Format("Change: {0}, Journal: {1}, Upgrade: {2}", this.Change, this.Journal, this.Upgrade, this.MaxCommitChange);
        }
    }
}
