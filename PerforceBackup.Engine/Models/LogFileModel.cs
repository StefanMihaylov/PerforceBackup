namespace PerforceBackup.Engine.Models
{
    using System.Globalization;

    public class LogFileModel
    {
        public string LogFileName { get; set; }

        public bool IsExist { get; set; }

        public double FileSize { get; set; }

        public bool IsCompress { get; set; }

        public override string ToString()
        {
            var fileNameAsTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.LogFileName.ToLower());
            var compressedMessage = this.IsCompress ? string.Format("{0} file compressed!", fileNameAsTitle) : string.Empty;
            var completeMessage =  string.Format("\"{0}\" file size is {1:0.00} Mb. {2}", fileNameAsTitle, this.FileSize, compressedMessage);

            return this.IsExist ? completeMessage : string.Format("File {0} do not exist!", fileNameAsTitle);
        }
    }
}
