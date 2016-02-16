namespace PerforceBackup.Engine.Excel
{
    public class ColumnSettings
    {
        public ColumnSettings(string name, string displayName, string type)
        {
            this.ColumnName = "@" + name;
            this.ColumnDisplayName = displayName;
            this.ColumnType = type;
        }

        public string ColumnName { get; private set; }

        public string ColumnDisplayName { get; private set; }

        public string ColumnType { get; private set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", this.ColumnDisplayName, this.ColumnType);
        }
    }
}
