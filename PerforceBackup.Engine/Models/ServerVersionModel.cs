namespace PerforceBackup.Engine.Models
{
    using System;

    public class ServerVersionModel
    {
        public string Platform { get; set; }

        public string Version { get; set; }

        public string Revision { get; set; }

        public DateTime Date { get; set; }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2} - {3}", this.Platform, this.Version, this.Revision, this.Date.ToShortDateString());
        }
    }
}
