namespace PerforceBackup.Engine.Interfaces
{
    using System;

    public interface IResultLogger
    {
        void WriteInfo(string message);

        void WriteInfoFormat(string format, params object[] param);

        void WriteError(Exception error);
    }
}
