namespace PerforceBackup.Engine
{
    using PerforceBackup.Engine.Interfaces;
    using System.Configuration;
    using System.IO;

    public class Configurations : IConfigurations, IArhiveSettings
    {
        public string RootPath { get { return ConfigurationManager.AppSettings["RootPath"]; } }

        public string ServerSubPath { get { return ConfigurationManager.AppSettings["ServerSubPath"]; } }

        public string DepotSubPath { get { return ConfigurationManager.AppSettings["DepotSubPath"]; } }

        public string JournalSubPath { get { return ConfigurationManager.AppSettings["JournalSubPath"]; } }

        public string CheckpointSubPath { get { return ConfigurationManager.AppSettings["CheckpointSubPath"]; } }

        public string BackupArhiveSubPath { get { return ConfigurationManager.AppSettings["BackupArhiveSubPath"]; } }

        public string SevenZipSubPath { get { return ConfigurationManager.AppSettings["SevenZipSubPath"]; } }

        public string LogFilesSubPath { get { return ConfigurationManager.AppSettings["LogFilesSubPath"]; } }

        public string MaxLogSize { get { return ConfigurationManager.AppSettings["MaxLogSize"]; } }

        public string MaxAuditLogSize { get { return ConfigurationManager.AppSettings["MaxAuditLogSize"]; } }    

        public string CheckpointLogName { get { return ConfigurationManager.AppSettings["CheckpointLogName"]; } }

        public string CheckpointLogExtension { get { return ConfigurationManager.AppSettings["CheckpointLogExtension"]; } }

        // directory arhive settings
        public string ArhivePath { get { return ConfigurationManager.AppSettings["ArhivePath"]; } }

        public string ArhiveName { get { return ConfigurationManager.AppSettings["ArhiveName"]; } }

        public string ArhiveType { get { return ConfigurationManager.AppSettings["ArhiveType"]; } }

        // conmined paths
        public string LogArhivePath
        {
            get
            {
                return Path.Combine(this.RootPath, this.BackupArhiveSubPath);
            }
        }

        public string DepotPath
        {
            get
            {
                return Path.Combine(this.RootPath, this.DepotSubPath);
            }
        }

        public string BackupArhivePath
        {
            get
            {
                return Path.Combine(this.RootPath, this.BackupArhiveSubPath);
            }
        }
    }
}
