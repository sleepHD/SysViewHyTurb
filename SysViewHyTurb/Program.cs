

namespace SysViewHyTurb
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using data;
    using Unosquare.Labs.EmbedIO;
    using Unosquare.Labs.EmbedIO.Modules;

    static class Program
    {

        /// <summary>
        /// Gets the config file path.
        /// </summary>
        /// <value>
        /// The config file path.
        /// </value>
        public static string ConfigFilePath
        {
            get
            {
                var assemblyPath = Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location);

#if DEBUG
                // This lets you edit the files without restarting the server.
                return Directory.GetParent(assemblyPath).Parent.FullName + "\\SysView.xml";
#else
                // This is when you have deployed the server.
                return Path.Combine(assemblyPath, "html");
#endif
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var doc = XDocument.Load(ConfigFilePath);
            var repo = new DataRepo(doc);
            while (repo.AppNum > 0)
            {
                
            }
            //Application.Exit();
        }
    }
}
