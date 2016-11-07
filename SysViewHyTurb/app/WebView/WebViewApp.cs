using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using SysViewHyTurb.data;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace SysViewHyTurb.app.WebView
{
    public class WebViewApp
    {
        private readonly string url;

        private readonly DataRepo repo;

        private readonly string htmlRootPath;

        private void DataChanged(KeyValuePair<string, object>[] keyValues)
        {
            var message = new StringBuilder();
            foreach(var keyValue in keyValues)
            {
                message.Append(keyValue.Key + " : " + keyValue.Value + Environment.NewLine);
            }
            MessageBox.Show(message.ToString(), "data");
        }

        /// <summary>
        /// Gets the HTML root path.
        /// </summary>
        /// <value>
        /// The HTML root path.
        /// </value>
        public static string HtmlRootPath
        {
            get
            {
                var assemblyPath = Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location);

#if DEBUG
                // This lets you edit the files without restarting the server.
                return Path.Combine(Directory.GetParent(assemblyPath).Parent.FullName, "html");
#else
                // This is when you have deployed the server.
                return Path.Combine(assemblyPath, "html");
#endif
            }
        }

        public WebViewApp(XElement webViewElement, DataRepo repo)
        {
            this.url = webViewElement.Attribute("url").Value;
            this.repo = repo;
            this.htmlRootPath = webViewElement.Attribute("htmlRootPath").Value;

            // Our web server is disposable. Note that if you don't want to use logging,
            // there are alternate constructors that allow you to skip specifying an ILog object.
            var embedWebServer = new WebServer(this.url);

            // First, we will configure our web server by adding Modules.
            // Please note that order DOES matter.
            // ================================================================================================
            // If we want to enable sessions, we simply register the LocalSessionModule
            // Beware that this is an in-memory session storage mechanism so, avoid storing very large objects.
            // You can use the server.GetSession() method to get the SessionInfo object and manupulate it.
            // You could potentially implement a distributed session module using something like Redis
            embedWebServer.RegisterModule(new LocalSessionModule());

            // Here we setup serving of static files
            embedWebServer.RegisterModule(new StaticFilesModule(HtmlRootPath));
            // The static files module will cache small files in ram until it detects they have been modified.
            embedWebServer.Module<StaticFilesModule>().UseRamCache = true;
            embedWebServer.Module<StaticFilesModule>().DefaultExtension = ".html";
            // We don't need to add the line below. The default document is always index.html.
            //EmbedWebServer.Module<StaticFilesModule>().DefaultDocument = "SysViewHyTurb_main.html";

            embedWebServer.RegisterModule(new WebSocketsModule());
            embedWebServer.Module<WebSocketsModule>().RegisterWebSocketsServer<WebSocketsScadaServer>("/scada");

            // Once we've registered our modules and configured them, we call the RunAsync() method.
            // This is a non-blocking method (it return immediately) so in this case we avoid
            // disposing of the object until a key is pressed.
            //server.Run();
            embedWebServer.RunAsync();
            this.repo.ValueChanged += this.DataChanged;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var frm = new MainForm();
            frm.Navigate(this.url);
            this.repo.AppNum = 1;
            Application.Run(frm);
            this.repo.AppNum = 0;
        }
    }
}
