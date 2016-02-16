namespace PerforceBackup.Engine.Models
{
    using System;

    public class CheckPointLogModel
    {
        public DateTime StartDate { get; set; }

        public CountersModel Counters { get; set; }

        public LogFileModel Log { get; set; }

        public LogFileModel AuditLog { get; set; }

        public ArhiveModel Arhive { get; set; }

        public SizesModel Sizes { get; set; }

        public double DepotSize { get; set; }

        public int Users { get; set; }

        public ServerVersionModel ServerInfo { get; set; }
    }
}
