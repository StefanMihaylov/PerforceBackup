namespace PerforceBackup.Engine.Logger
{
    using System;
    using PerforceBackup.Engine.Interfaces;

    public class ConsoleLogger : IInfoLogger
    {
        public void Write(string format, params object[] arg)
        {
            Console.Write(format, arg);
        }

        public void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}
