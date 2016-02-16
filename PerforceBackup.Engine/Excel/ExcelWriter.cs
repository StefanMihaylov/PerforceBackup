namespace PerforceBackup.Engine.Excel
{
    using System;
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.IO;
    using System.Linq;

    using PerforceBackup.Engine.Common;
    using PerforceBackup.Engine.Models;

    public class ExcelWriter
    {
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

        public void AddRow(CheckPointLogModel logData)
        {
            //string excelConnectionString = @"Provider={0};Data Source=..\..\SalariesReport.xlsx; Extended Properties=""Excel 12.0 Xml;HDR=YES""";

            var excelConnectionString = this.GetConnectionString();
            using (var connection = new OleDbConnection(excelConnectionString))
            {
                var columns = new List<Cell>();
                columns.Add(new Cell("checkpoint", "№", "int", logData.Counters.Journal));
                columns.Add(new Cell("date", "Дата", "nvarchar(30)", this.TruncateDate(logData.StartDate).ToString(StringConstrants.DateTimeFormat)));
                columns.Add(new Cell("changelist", "Changelist", "int", logData.Counters.MaxCommitChange));
                columns.Add(new Cell("upgrade", "Upgrade", "int", logData.Counters.Upgrade));
                columns.Add(new Cell("users", "Потребители", "int", logData.Users));
                columns.Add(new Cell("files", "Файлове", "int", logData.Sizes.FilesCount));
                columns.Add(new Cell("revisions", "Версии", "int", logData.Sizes.RevisionsCount));
                columns.Add(new Cell("depot", "Депо", "nvarchar(20)", string.Format("{0:0.00} Mb", logData.DepotSize)));
                columns.Add(new Cell("log", "Log", "nvarchar(20)", string.Format("{0:0.00} Mb", logData.Log.FileSize)));
                columns.Add(new Cell("auditlog", "Auditlog", "nvarchar(20)", string.Format("{0:0.00} Mb", logData.AuditLog.FileSize)));
                columns.Add(new Cell("arhive", "Име", "nvarchar(30)", logData.Arhive != null ? logData.Arhive.ArhivePatternName : string.Empty));
                columns.Add(new Cell("arhiveSize", "Размер", "nvarchar(20)", string.Format("{0:0.00} Mb", logData.Arhive != null ? logData.Arhive.Size : 0)));
                columns.Add(new Cell("serverPlatform", "Платформа", "nvarchar(30)", logData.ServerInfo.Platform));
                columns.Add(new Cell("serverVersion", "Версия", "nvarchar(30)", logData.ServerInfo.Version));
                columns.Add(new Cell("serverRevision", "Ревизия", "int", logData.ServerInfo.Revision));
                columns.Add(new Cell("serverDate", "от Дата", "nvarchar(20)", this.TruncateDate(logData.ServerInfo.Date).ToString(StringConstrants.DateFormat)));

                connection.Open();

                var sheetName = "Checkpoint_Log";// +"$";

                var sheets = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
                var isSheetExist = false;
                for (int i = 0; i < sheets.Rows.Count; i++)
                {
                    var currentSheetName = sheets.Rows[i]["TABLE_NAME"].ToString();
                    if (currentSheetName.Equals(sheetName))
                    {
                        isSheetExist = true;
                    }
                }

                if (!isSheetExist)
                {
                    var columnSettings = string.Join(", ", columns.Select(r => r.ColumnSettings.ToString()));
                    var createSheetQuery = string.Format("CREATE TABLE [{0}] ({1})", sheetName, columnSettings);
                    OleDbCommand createSheet = new OleDbCommand(createSheetQuery, connection);
                    createSheet.ExecuteNonQuery();
                }

                var columnDisplayNames = string.Join(", ", columns.Select(r => string.Format("[{0}]", r.ColumnSettings.ColumnDisplayName)));
                var columnNames = string.Join(", ", columns.Select(r => r.ColumnSettings.ColumnName));
                var inserQuery = string.Format("INSERT INTO [{0}] ({1}) VALUES ({2})", sheetName, columnDisplayNames, columnNames);
                var insertLogData = new OleDbCommand(inserQuery, connection);
                foreach (var item in columns)
                {
                    insertLogData.Parameters.AddWithValue(item.ColumnSettings.ColumnName, item.CellData);
                }

                insertLogData.ExecuteNonQuery();
            }
        }

        private string GetConnectionString()
        {
            var properties = new Dictionary<string, string>();
            properties.Add("Data Source", this.FullFileRoot);

            var excelFile = new FileInfo(this.FullFileRoot);

            switch (excelFile.Extension)
            {
                case ".xlsx":
                    // XLSX - Excel 2007, 2010, 2012, 2013
                    properties.Add("Provider", "Microsoft.ACE.OLEDB.12.0");
                    properties.Add("Extended Properties", "Excel 12.0 XML");
                    break;
                case ".xls":
                    // XLS - Excel 2003 and Older
                    properties.Add("Provider", "Microsoft.Jet.OLEDB.4.0");
                    properties.Add("Extended Properties", "Excel 8.0");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Only 'xlsx' and 'xls' file extensions are allowed! Not '" + excelFile.Extension + "'!");
            }

            var connectionString = string.Join(";", properties.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));
            return connectionString;
        }

        private DateTime TruncateDate(DateTime date)
        {
            var truncatedDateTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            return truncatedDateTime;
        }
    }
}