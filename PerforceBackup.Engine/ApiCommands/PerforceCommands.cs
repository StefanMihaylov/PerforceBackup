namespace PerforceBackup.Engine.ApiCommands
{
    using Perforce.P4;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using PerforceBackup.Engine.Interfaces;
    using PerforceBackup.Engine.Models;
    using PerforceBackup.Engine.Common;

    public class PerforceCommands : PerforceConnection, IPerforceCommands
    {
        public PerforceCommands(string uri, IInfoLogger logger)
            : base(uri)
        {
            this.InfoLogger = logger;
        }

        protected IInfoLogger InfoLogger { get; private set; }

        public string ServerRoot
        {
            get
            {
                var serverRoot = this.Repository.Server.Metadata.Root;
                return serverRoot;
            }
        }

        public CountersModel GetCounters()
        {
            Options options = new Options();
            CountersModel result = new CountersModel()
            {
                Change = long.Parse(this.Repository.GetCounter("change", options).Value),
                Journal = long.Parse(this.Repository.GetCounter("journal", options).Value),
                Upgrade = long.Parse(this.Repository.GetCounter("upgrade", options).Value),
                MaxCommitChange = long.Parse(this.Repository.GetCounter("MaxCommitChange", options).Value),
            };

            this.InfoLogger.WriteLine(" - Counters: {0}", result);
            return result;
        }

        public int GetProjectCount()
        {
            IList<string> dirs = this.Repository.GetDepotDirs(null, "//depot/*");
            int count = dirs.Count();

            this.InfoLogger.WriteLine(" - Projects: {0}", count);
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

            this.InfoLogger.WriteLine(" - Sizes: {0}", result);
            return result;
        }

        public bool Verify()
        {
            this.InfoLogger.Write(" - Validate: ");

            this.Repository.Connection.CommandTimeout = TimeSpan.FromMinutes(5);

            P4Command command = new P4Command(this.Repository, "verify", false, "//...");
            P4CommandResult commandsResults = command.Run(new StringList(new string[] { "-q" }));

            if (!commandsResults.Success)
            {
                this.InfoLogger.WriteLine(StringConstrants.ErrorMessage);
                throw new OperationCanceledException("Verification failed");
            }
            else
            {
                this.Repository.Connection.CommandTimeout = TimeSpan.FromSeconds(5);
                this.InfoLogger.WriteLine(StringConstrants.SuccessMessage);

                return true;
            }
        }

        public bool StopServer()
        {
            this.InfoLogger.Write(" - Stop Server: ");

            var serverStatus = this.Repository.Server.State;
            if (serverStatus == ServerState.Offline)
            {
                this.InfoLogger.WriteLine(StringConstrants.SuccessMessage);
                return true;
            }

            P4Command command = new P4Command(this.Repository, "admin", false, "stop");

            P4CommandResult commandsResults = command.Run();

            if (commandsResults.Success)
            {
                this.InfoLogger.WriteLine(StringConstrants.SuccessMessage);
                return true;
            }
            else
            {
                this.InfoLogger.WriteLine(StringConstrants.ErrorMessage);
                throw new OperationCanceledException("Server not stoped");
            }
        }

        public int GetUsersCount()
        {
            IList<User> usersCollection = this.Repository.GetUsers(new Options());
            int count = usersCollection.Count;

            this.InfoLogger.WriteLine(" - Users: {0}", count);
            return count;
        }

        public ServerVersionModel GetServerVersion()
        {
            ServerVersion metadata = this.Repository.Server.Metadata.Version;
            ServerVersionModel result = new ServerVersionModel()
            {
                Platform = metadata.Platform,
                Revision = metadata.Minor,
                Version = metadata.Major,
                Date = metadata.Date, // error
            };

            var versionString = this.Repository.Server.Metadata.RawData["serverVersion"];
            var versionParts = versionString.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            result.Date = DateTime.Parse(versionParts[1]);

            this.InfoLogger.WriteLine(" - Server: {0}", result);
            return result;
        }
    }
}
