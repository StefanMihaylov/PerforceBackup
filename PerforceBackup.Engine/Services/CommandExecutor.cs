namespace PerforceBackup.Engine.Services
{   
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    using log4net;

    public abstract class CommandExecutor
    {
        private string perforcePath;
        private string exeSubpath;
        private string exeName;
        private ILog logger;

        protected CommandExecutor(ILog logger, string path, string exeSubpath, string exeName)
        {
            this.logger = logger;
            this.PerforcePath = path;
            this.ExeSubpath = exeSubpath;
            this.ExeName = exeName;
        }

        protected string PerforcePath
        {
            get
            {
                return this.perforcePath;
            }

            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("Perforce path cannot be NULL");
                }

                this.perforcePath = value;
            }
        }

        private string ExeSubpath
        {
            get
            {
                return this.exeSubpath;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("CommandLine Client subpath cannot be NULL");
                }

                this.exeSubpath = value;
            }
        }

        private string ExeName
        {
            get
            {
                return this.exeName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("Exe name cannot be NULL");
                }

                this.exeName = value;
            }
        }

        protected string ExePath
        {
            get
            {
                return Path.Combine(this.PerforcePath, this.ExeSubpath);
            }
        }

        protected string ExeFullPath
        {
            get
            {
                return Path.Combine(this.ExePath, this.exeName);
            }
        }

        protected virtual string ExecuteCommand(string command)
        {
            return ExecuteCommand(command, string.Empty);
        }

        protected virtual string ExecuteCommand(string command, string arguments)
        {
            var stdOutput = new StringBuilder();

            var exeProcess = new Process();
            var fullCommand = string.Format("{0} {1}", command.Trim(), arguments.Trim()).Trim();
            exeProcess.StartInfo = this.GetExeProcess(fullCommand);
            exeProcess.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data);

            string stdError = null;
            try
            {
                exeProcess.Start();
                exeProcess.BeginOutputReadLine();
                stdError = exeProcess.StandardError.ReadToEnd();
                exeProcess.WaitForExit();
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("OS error while executing {0}:{1}", this.Format(command, arguments), e.Message);
                throw new Exception(errorMessage, e);
            }

            if (exeProcess.ExitCode == 0)
            {
                var result = stdOutput.ToString().Trim();

                var formatter = result.Length < 50 ? string.Empty: Environment.NewLine + "\t";
                this.logger.Info(string.Format("{0} => {2}{1}", fullCommand, result, formatter));

                return result;
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine(string.Format("Std output: {0}", stdOutput.ToString()));
                }

                var errorMessage = string.Format("{0} finished with exit code = {1}:{2}", this.Format(command, arguments), exeProcess.ExitCode, message);
                throw new Exception(errorMessage);
            }

        }

        private string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }

        private ProcessStartInfo GetExeProcess(string command)
        {
            var startInfo = new ProcessStartInfo()
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = this.PerforcePath,
                FileName = this.ExeFullPath,
                Arguments = command
            };

            return startInfo;
        }
    }
}
