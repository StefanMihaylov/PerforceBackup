namespace PerforceBackup.Engine.Logger
{
    using System;
    using log4net;
    using PerforceBackup.Engine.Interfaces;

    public class ResultLogger : IResultLogger
    {
        public const string LoggerName = "PerforceBackup";

        private ILog logger;

        public ResultLogger(string Log4NetPath)
        {
            this.logger = LogManager.GetLogger(LoggerName);
            Log4netSettings.Setup(Log4NetPath);
        }

        public void WriteInfo(string message)
        {
            this.logger.Info(message);
        }

        public void WriteInfoFormat(string format, params object[] param)
        {
            this.logger.InfoFormat(format, param);
        }

        public void WriteError(Exception error)
        {
            this.logger.Fatal(error);
        }
    }
}
