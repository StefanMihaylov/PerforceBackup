namespace PerforceBackup.Engine.Excel
{
    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;

    using Spire.Xls;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class ExcelWriter : IExcelWriter
    {
        private const string SheetName = "Checkpoint_Log";

        private string root;
        private string fileName;
        private string extension;

        public ExcelWriter(string root, string fileName, string extension)
        {
            this.root = root;
            this.fileName = fileName;
            this.extension = extension;
        }

        public string FullFileRoot
        {
            get
            {
                return Path.Combine(this.root, this.FullFileName);
            }
        }

        private string FullFileName
        {
            get
            {
                return string.Format("{0}.{1}", fileName, extension);
            }
        }

        public string PdfFullFileRoot
        {
            get
            {
                return Path.Combine(this.root, this.PdfFullFileName);
            }
        }

        private string PdfFullFileName
        {
            get
            {
                return string.Format("{0}.pdf", fileName);
            }
        }
        public void AddRow(CheckPointLogModel logData)
        {
            var columns = new List<KeyValuePair<string, object>>();

            if (logData != null)
            {
                columns.Add(new KeyValuePair<string, object>("№", logData.Counters.Journal));
                columns.Add(new KeyValuePair<string, object>("Дата", this.TruncateDate(logData.StartDate).ToString(StringConstrants.DateTimeFormat)));
                columns.Add(new KeyValuePair<string, object>("Changelist", logData.Counters.MaxCommitChange));
                columns.Add(new KeyValuePair<string, object>("Upgrade", logData.Counters.Upgrade));
                columns.Add(new KeyValuePair<string, object>("Потребители", logData.UserCount));
                columns.Add(new KeyValuePair<string, object>("Проекти", logData.ProjectCount));
                columns.Add(new KeyValuePair<string, object>("Файлове", logData.Sizes.FilesCount));
                columns.Add(new KeyValuePair<string, object>("Версии", logData.Sizes.RevisionsCount));
                columns.Add(new KeyValuePair<string, object>("Депо", string.Format("{0:0.00} Mb", logData.DepotSize)));
                columns.Add(new KeyValuePair<string, object>("Log", string.Format("{0:0.00} Mb", logData.Log.FileSize)));
                columns.Add(new KeyValuePair<string, object>("Auditlog", string.Format("{0:0.00} Mb", logData.AuditLog.FileSize)));
                columns.Add(new KeyValuePair<string, object>("Име", logData.Arhive != null ? logData.Arhive.ArhivePatternName : string.Empty));
                columns.Add(new KeyValuePair<string, object>("Размер", string.Format("{0:0.00} Mb", logData.Arhive != null ? logData.Arhive.Size : 0)));
                columns.Add(new KeyValuePair<string, object>("Платформа", logData.ServerInfo.Platform));
                columns.Add(new KeyValuePair<string, object>("Версия", string.Format("'{0}", logData.ServerInfo.Version)));
                columns.Add(new KeyValuePair<string, object>("Ревизия", logData.ServerInfo.Revision));
                columns.Add(new KeyValuePair<string, object>("от Дата", this.TruncateDate(logData.ServerInfo.Date).ToString(StringConstrants.DateFormat)));
            }

            var workbook = new Workbook();
            workbook.LoadFromFile(this.FullFileRoot);
            Worksheet sheet = workbook.Worksheets.FirstOrDefault(s => s.Name == SheetName) as Worksheet;
            if (sheet == null)
            {
                throw new ArgumentNullException("Excel sheet not found");
            }

            var range = sheet.Range;
            var headerStart = this.FindHeader(range, "№");
            var lastRow = this.FindLastRow(range, headerStart);
            sheet.InsertRow(lastRow.Row, 1, InsertOptionsType.FormatAsBefore);

            for (int index = 0; index < columns.Count; index++)
            {
                var currentCol = lastRow.Column + index;
                var headerText = sheet.Range[headerStart.Row, currentCol].Text;
                var currentCell = columns[index];
                if (headerText == currentCell.Key)
                {
                    sheet.Range[lastRow.Row, currentCol].Value2 = currentCell.Value;
                }
            }

            workbook.Save();
            workbook.Dispose();
        }

        public void ConvertToPdf()
        {
            // load Excel file
            var workbook = new Workbook();
            workbook.LoadFromFile(this.FullFileRoot);

            // Save PDF
            if (File.Exists(this.PdfFullFileRoot))
            {
                File.Delete(this.PdfFullFileRoot);
            }

            workbook.SaveToFile(this.PdfFullFileRoot, FileFormat.PDF);
        }

        public void OpenExcel()
        {
            Process.Start(this.FullFileRoot);
        }

        public void OpenPdf()
        {
            Process.Start(this.PdfFullFileRoot);
        }

        private CellPosition FindHeader(CellRange range, string startValue)
        {
            var headerStart = new CellPosition(0, 0);
            for (int row = range.Row; row <= range.LastRow; row++)
            {
                for (int col = range.Column; col <= range.LastColumn; col++)
                {
                    var currentCell = range[row, col].Value;
                    if (currentCell == startValue)
                    {
                        headerStart = new CellPosition(row, col);
                        break;
                    }
                }
            }

            return headerStart;
        }

        private CellPosition FindLastRow(CellRange range, CellPosition headerStart)
        {
            var result = headerStart;
            for (int row = headerStart.Row; row <= range.LastRow + 1; row++)
            {
                var currentCell = range[row, headerStart.Column].Value;
                if (string.IsNullOrWhiteSpace(currentCell))
                {
                    result.Row = row;
                    break;
                }
            }

            return result;
        }

        private DateTime TruncateDate(DateTime date)
        {
            var truncatedDateTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            return truncatedDateTime;
        }
    }
}
