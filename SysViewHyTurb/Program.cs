

namespace SysViewHyTurb
{
    using System;
    using System.Windows.Forms;
    using Unosquare.Labs.EmbedIO;
    using Unosquare.Labs.EmbedIO.Log;
    using Unosquare.Labs.EmbedIO.Modules;
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            
            Url = "http://localhost:9696/";
            if (args.Length > 0)
                Url = args[0];

            // Our web server is disposable. Note that if you don't want to use logging,
            // there are alternate constructors that allow you to skip specifying an ILog object.
            EmbedWebServer = new WebServer(Url);

            // First, we will configure our web server by adding Modules.
            // Please note that order DOES matter.
            // ================================================================================================
            // If we want to enable sessions, we simply register the LocalSessionModule
            // Beware that this is an in-memory session storage mechanism so, avoid storing very large objects.
            // You can use the server.GetSession() method to get the SessionInfo object and manupulate it.
            // You could potentially implement a distributed session module using something like Redis
            EmbedWebServer.RegisterModule(new LocalSessionModule());

            // Here we setup serving of static files
            EmbedWebServer.RegisterModule(new StaticFilesModule("./WebView"));
            // The static files module will cache small files in ram until it detects they have been modified.
            EmbedWebServer.Module<StaticFilesModule>().UseRamCache = true;
            EmbedWebServer.Module<StaticFilesModule>().DefaultExtension = ".html";
            // We don't need to add the line below. The default document is always index.html.
            //EmbedWebServer.Module<StaticFilesModule>().DefaultDocument = "SysViewCp_main.html";

            EmbedWebServer.RegisterModule(new WebSocketsModule());
            EmbedWebServer.Module<WebSocketsModule>().RegisterWebSocketsServer<WebSocketsScadaServer>("/scada");   

            // Once we've registered our modules and configured them, we call the RunAsync() method.
            // This is a non-blocking method (it return immediately) so in this case we avoid
            // disposing of the object until a key is pressed.
            //server.Run();
            EmbedWebServer.RunAsync();
           

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

       public static string Url;
       public static WebServer EmbedWebServer;
    }
}
