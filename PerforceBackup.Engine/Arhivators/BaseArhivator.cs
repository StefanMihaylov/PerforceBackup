namespace PerforceBackup.Engine.Arhivators
{
    using PerforceBackup.Engine.Interfaces;
    using System;
    using System.IO;

    public abstract class BaseArhivator
    {
        private IArhivator arhivator;
        private string subPath;

        public BaseArhivator(IArhivator arhivator, IInfoLogger logger, string rootPath, string subPath)
        {
            this.Arhivator = arhivator;
            this.Logger = logger;
            this.RootPath = rootPath;
            this.subPath = subPath;
        }

        protected IArhivator Arhivator
        {
            get { return this.arhivator; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Arhivator cannot be null!");
                }

                this.arhivator = value;
            }
        }

        protected IInfoLogger Logger { get; set; }

        protected string RootPath { get; set; }

        protected string CombinedPath
        {
            get
            {
                return Path.GetFullPath(Path.Combine(this.RootPath, this.subPath));
            }
        }
    }
}
