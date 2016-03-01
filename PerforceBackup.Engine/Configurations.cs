namespace PerforceBackup.Engine
{
    using PerforceBackup.Engine.Interfaces;
    using System.Configuration;
    using System.Linq;
    using System.IO;

    public class Configurations : IConfigurations
    {
        public const string ConfigFileName = "Configurations.xml";

        private KeyValueConfigurationCollection settings;

        public Configurations()
        {
            var fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = ConfigFileName;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            this.settings = config.AppSettings.Settings;
        }

        public string ServerUrl { get { return this.settings["ServerUrl"].Value; } }

        public string ServerUser { get { return this.settings["ServerUser"].Value; } }

        public string UserWorkSpace { get { return this.settings["UserWorkSpace"].Value; } }


        public string DepotSubPath { get { return this.settings["DepotSubPath"].Value; } }

        public string BackupSubPath { get { return this.settings["BackupSubPath"].Value; } }

        public string CheckpointSubPath { get { return this.settings["CheckpointSubPath"].Value; } }

        public string LogArhiveSubPath { get { return this.settings["LogArhiveSubPath"].Value; } }

        public string CheckpointArhiveSubPath { get { return this.settings["CheckpointArhiveSubPath"].Value; } }

        public string CheckpointLogPath { get { return this.settings["CheckpointLogPath"].Value; } }

        public string MaxLogSize { get { return this.settings["MaxLogSize"].Value; } }

        public string MaxAuditLogSize { get { return this.settings["MaxAuditLogSize"].Value; } }

        public string RootArhiveSubPath { get { return this.settings["RootArhiveSubPath"].Value; } }

        public string ArhivePath { get { return this.settings["ArhivePath"].Value; } }
    }
}
