namespace PerforceBackup.Engine.Excel
{
    public struct CellPosition
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public CellPosition(int row, int column)
            : this()
        {
            this.Row = row;
            this.Column = column;
        }
    }
}
