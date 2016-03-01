namespace PerforceBackup.Engine.Logger
{
    using PerforceBackup.Engine.Interfaces;

    public class EmptyInfoLogger: IInfoLogger
    {
        public void Write(string format, params object[] arg)
        {
        }

        public void WriteLine(string format, params object[] arg)
        {
        }
    }
}
