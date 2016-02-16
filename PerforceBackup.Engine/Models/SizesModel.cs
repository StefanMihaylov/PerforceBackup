namespace PerforceBackup.Engine.Models
{
    public class SizesModel
    {
        public long FilesCount { get; set; }

        public long FileSize { get; set; }

        public long RevisionsCount { get; set; }

        public long RevisionsSize { get; set; }

        public override string ToString()
        {
            return string.Format("{0} / {1} : {2} / {3}", this.FilesCount, this.RevisionsCount, this.FileSize, this.RevisionsSize);
        }
    }
}
