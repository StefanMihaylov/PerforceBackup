namespace PerforceBackup.ApiEngine
{
    using Perforce.P4;
    using PerforceBackup.ApiEngine.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;

    public class PerforceCommands : PerforceConnection
    {
        public PerforceCommands(string uri, string user, string ws_client)
            : base(uri, user, ws_client)
        {
        }

        public CountersModel GetCounters()
        {
            var options = new Options();
            var result = new CountersModel()
            {
                Change = long.Parse(this.Repository.GetCounter("change", options).Value),
                Journal = long.Parse(this.Repository.GetCounter("journal", options).Value),
                Upgrade = long.Parse(this.Repository.GetCounter("upgrade", options).Value),
                MaxCommitChange = long.Parse(this.Repository.GetCounter("MaxCommitChange", options).Value),
            };

            return result;
        }

        public int GetProjectCount()
        {
            IList<string> dirs = this.Repository.GetDepotDirs(null, "//depot/*");
            int count = dirs.Count();
            return count;
        }

        public SizesModel GetSizes()
        {
            P4Command command = new P4Command(this.Repository, "sizes", true, "//depot/...");

            P4CommandResult fileCommandsResults = command.Run(new StringList(new string[] { "-z" }));
            P4CommandResult revisionCommandsResults = command.Run(new StringList(new string[] { "-z", "-a" }));

            var result = new SizesModel()
            {
                FileSize = long.Parse(fileCommandsResults.TaggedOutput[0]["fileSize"]),
                FilesCount = long.Parse(fileCommandsResults.TaggedOutput[0]["fileCount"]),
                RevisionsSize = long.Parse(revisionCommandsResults.TaggedOutput[0]["fileSize"]),
                RevisionsCount = long.Parse(revisionCommandsResults.TaggedOutput[0]["fileCount"]),
            };

            return result;
        }

        public bool Verify()
        {
            this.Repository.Connection.CommandTimeout = TimeSpan.FromMinutes(5);
            P4Command command = new P4Command(this.Repository, "verify", false, "//...");

            P4CommandResult commandsResults = command.Run(new StringList(new string[] { "-q" }));

            if (!commandsResults.Success)
            {
                throw new OperationCanceledException("Verification failed");
            }
            else
            {
                this.Repository.Connection.CommandTimeout = TimeSpan.FromSeconds(5);
                return true;
            }
        }

        public void StartServer()
        {
            ServiceController service = new ServiceController("Perforce");
            try
            {
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(1));
                }
            }
            catch
            {
                throw new OperationCanceledException("Server not started");
            }
        }

        public bool StopServer()
        {
            var serverStatus = this.Repository.Server.State;
            if (serverStatus == ServerState.Offline)
            {
                return true;
            }

            P4Command command = new P4Command(this.Repository, "admin", false, "stop");

            P4CommandResult commandsResults = command.Run();

            if (!commandsResults.Success)
            {
                throw new OperationCanceledException("Server not stoped");
            }
            else
            {
                return true;
            }
        }

        public int GetUsersCount()
        {
            var options = new Options();
            var result = this.Repository.GetUsers(options);
            return result.Count;
        }

        public ServerVersionModel GetServerVersion()
        {
            var metadata = this.Repository.Server.Metadata.Version;
            var result = new ServerVersionModel()
            {
                Platform = metadata.Platform,
                Revision = metadata.Major,
                Version = metadata.Minor,
                Date = metadata.Date, // error
            };

            return result;
        }
    }
}
