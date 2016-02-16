namespace PerforceBackup.Engine
{
    using log4net;
using System;
    public class CommandPromptExecutor : CommandExecutor
    {
        public CommandPromptExecutor(ILog logger)
            : base(logger, @"C:\Windows\System32\", string.Empty, "cmd.exe")
        {
        }

        public bool StartService(string serviceName, string successString)
        {
            var result = this.ExecuteCommand("net start", serviceName);

            if (result != null && result.Contains(successString))
            {
                return true;
            }
            else
            {
                throw new OperationCanceledException(result);
            }
        }

        protected override string ExecuteCommand(string command)
        {
            return this.ExecuteCommand(command, string.Empty);
        }

        protected override string ExecuteCommand(string command, string arguments)
        {
            return base.ExecuteCommand(string.Format("/c {0}", command), arguments);
        }
    }
}
