namespace PerforceBackup.Engine.Models
{
    public class CheckpointModel
    {
        public bool IsCheckPointExist { get; set; }

        public string CheckpointName { get; set; }

        public bool IsOldCheckpointsCompressed { get; set; }
    }
}
