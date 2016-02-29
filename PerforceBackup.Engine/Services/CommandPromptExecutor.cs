namespace PerforceBackup.Engine.Services
{
    using log4net;
    using PerforceBackup.Engine.Base;
    using PerforceBackup.Engine.Interfaces;
    using System;
    using System.ServiceProcess;
    using PerforceBackup.Engine.Common;

    public class CommandPromptExecutor : CommandExecutor, ICommandPromptExecutor
    {
        private IInfoLogger infologger;

        public CommandPromptExecutor(ILog logger, IInfoLogger infologger)
            : base(logger, @"C:\Windows\System32\", string.Empty, "cmd.exe")
        {
            this.infologger = infologger;
        }

        public bool StartService(string serviceName = "Perforce")
        {
            this.infologger.Write(" - Start Server: ");

            var serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10.0));

                //string result = this.ExecuteCommand("net start", serviceName);

                //if (result != null && result.Contains(successString))
                //{
                //    this.infologger.WriteLine(StringConstrants.SuccessMessage);
                //    return true;
                //}
                //else
                //{
                // this.infologger.WriteLine(StringConstrants.ErrorMessage);
                //    throw new OperationCanceledException(result);
                //}
            }

            this.infologger.WriteLine(StringConstrants.SuccessMessage);
            return true;
        }

        public bool StopService(string serviceName = "Perforce")
        {
            this.infologger.Write(" - Stop Server: ");

            var serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10.0));
            }

            this.infologger.WriteLine(StringConstrants.SuccessMessage);
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
