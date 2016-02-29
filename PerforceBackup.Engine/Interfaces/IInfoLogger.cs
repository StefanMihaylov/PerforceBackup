namespace PerforceBackup.Engine.Interfaces
{
    public interface IInfoLogger
    {
        void Write(string format, params object[] arg);

        void WriteLine(string format, params object[] arg);
    }
}
