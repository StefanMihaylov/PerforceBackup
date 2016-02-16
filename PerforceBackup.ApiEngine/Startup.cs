using System;
using System.Reflection;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(PerforceBackup.ApiEngine.Startup), "Start")]

namespace PerforceBackup.ApiEngine
{
    public class Startup
    {
        public static void Start()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                // string resource = Array.Find(Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                string[] names = assembly.GetManifestResourceNames();
              //  string[] names2 = this.GetType().Assembly.GetManifestResourceNames();

                string resource = "EmbedAssembly.p4bridge.dll";
                using (var stream = assembly.GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
    }
}
