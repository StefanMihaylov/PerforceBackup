namespace PerforceBackup.Engine.ApiCommands
{
    using Perforce.P4;
    using System;
    using System.Reflection;
    using PerforceBackup.Engine.Interfaces;
    using System.IO;

    public class PerforceConnection
    {
        public PerforceConnection(string uri)
        {
            this.Uri = uri;
            //this.User = user;
            //this.Client = ws_client;           

            this.Repository = this.Initialize();
            this.Connect();
        }

        protected string Uri { get; private set; }

        // protected string User { get; private set; }

        //protected string Client { get; private set; }

        protected Repository Repository { get; private set; }

        protected void Connect()
        {
            var connection = this.Repository.Connection;
            if (!connection.Connect(null))
            {
                throw new Exception("Connection failed");
            }
        }

        private Repository Initialize()
        {
            // define the server, repository and connection
            ServerAddress addrerss = new ServerAddress(this.Uri);
            Server server = new Server(addrerss);
            Repository repository = new Repository(server);
            // Connection connection = repository.Connection;

            // use the connection variables for this connection
            // connection.UserName = this.User;

            //if (!string.IsNullOrWhiteSpace(this.Client))
            //{
            //    connection.Client = new Client() { Name = this.Client };
            //}

            return repository;
        }
    }
}
