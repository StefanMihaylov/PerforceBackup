namespace PerforceBackup.Engine.Models
{
    using System.Globalization;

    public class ArhiveModel
    {
        public string ArhivePatternName { get; set; }

        public double Size { get; set; }

        public string Path { get; set; }

        public string ArhiveFullPath { get; set; }

        public bool IsCompressed { get; set; }

        public override string ToString()
        {
           // var fileNameAsTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Name.ToLower());
            var completeMessage = string.Format("Done. \"{0}\" file size is {1:0.00} Mb", this.ArhivePatternName, this.Size);

            return this.IsCompressed ? completeMessage : "NO";
        }
    }
}
