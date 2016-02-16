namespace PerforceBackup.Engine.Base
{
    using PerforceBackup.Engine.Interfaces;
    using System;

    public abstract class BaseArhivator
    {
        private IArhivator arhivator;

        public BaseArhivator(IArhivator arhivator)
        {
            this.Arhivator = arhivator;
        }

        public IArhivator Arhivator
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
    }
}
