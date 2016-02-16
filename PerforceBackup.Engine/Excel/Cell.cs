namespace PerforceBackup.Engine.Excel
{
    public class Cell
    {
        public Cell(string columnName, string columnDisplayName, string columnType, object cellData)
        {
            this.ColumnSettings = new ColumnSettings(columnName, columnDisplayName, columnType);
            this.CellData = cellData;
        }

        public ColumnSettings ColumnSettings { get; private set; }

        public object CellData { get; private set; }
    }
}
