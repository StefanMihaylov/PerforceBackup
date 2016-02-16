namespace PerforceBackup.Engine.Services
{
    using log4net;
    using PerforceBackup.Engine.Base;
    using PerforceBackup.Engine.Interfaces;
    using System;
    using System.ServiceProcess;

    public class CommandPromptExecutor : CommandExecutor, ICommandPromptExecutor
    {
        public CommandPromptExecutor(ILog logger)
            : base(logger, @"C:\Windows\System32\", string.Empty, "cmd.exe")
        {
        }

        public bool StartService(string serviceName, string successString)
        {
            var serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Stopped)
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

            return true;
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
