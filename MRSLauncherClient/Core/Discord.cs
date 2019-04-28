using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;
using log4net;
using System.Threading;

namespace MRSLauncherClient
{
    public class Discord
    {
        #region Singleton

        private static Discord instance;
        public static Discord App
        {
            get
            {
                if (instance == null)
                    instance = new Discord();
                return instance;
            }
        }

        private Discord() { }

        #endregion

        public DiscordRpcClient client;
        private const string CLIENT_ID = "504570440317141013";
        Thread InvokeThread;
        bool IsWorking = false;

        private static ILog log = LogManager.GetLogger("Discord");

        public RichPresence Presence { get; set; }

        public void Initialize()
        {
            log.Info("Initializing Discord : " + CLIENT_ID);

            client = new DiscordRpcClient(CLIENT_ID);
            client.Logger = new DiscordRPC.Logging.NullLogger();

            client.OnReady += Client_OnReady;
            client.OnPresenceUpdate += Client_OnPresenceUpdate;

            log.Info("Initializing Client");
            client.Initialize();

            Presence = new RichPresence();
            Presence.Details = "Launcher";
            Presence.Assets = new Assets()
            {
                LargeImageKey = "mrsbig",
                LargeImageText = "Mystic Red Space"
            };

            IsWorking = true;
            InvokeThread = new Thread(Invoking);
            log.Info("Start Invoking Thread");
            InvokeThread.Start();
        }

        public void DeInitialize()
        {
            IsWorking = false;
            InvokeThread.Abort();
            client.Dispose();

            log.Info("DeInitialized");
        }

        private void Invoking()
        {
            while (IsWorking)
            {
                Thread.Sleep(150);

                client.SetPresence(Presence.Clone());
                client.Invoke();
            }
        }

        private void Client_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage args)
        {
            
        }

        private void Client_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
        {
            
        }
    }
}
