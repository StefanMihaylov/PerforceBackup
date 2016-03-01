namespace PerforceBackup.Engine.Logger
{
    using PerforceBackup.Engine.Interfaces;

    public class EmptyResultLogger: IResultLogger
    {
        public void WriteInfo(string message)
        {
        }

        public void WriteInfoFormat(string format, params object[] param)
        {
        }

        public void WriteError(System.Exception error)
        {
        }
    }
}
