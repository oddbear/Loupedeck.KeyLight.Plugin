namespace Loupedeck.KeyLightPlugin
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using Commands;

    using Makaretu.Dns;

    public class KeyLightPlugin : Plugin
    {
        private readonly HttpClient _httpClient;
        private readonly Thread _refreshThread;
        private readonly MulticastService _multicastService;

        internal KeyLightClient KeyLightClient { get; }
        internal ConcurrentDictionary<String, KeyLightState> KeyLights { get; } = new ConcurrentDictionary<String, KeyLightState>();

        //public override Boolean HasNoApplication => true;
        //public override Boolean UsesApplicationApiOnly => true;

        public KeyLightPlugin()
        {
            this._httpClient = new HttpClient();
            this.KeyLightClient = new KeyLightClient(this._httpClient);
            this._refreshThread = new Thread(this.RefreshKeyLightInfo) { IsBackground = true };
            this._multicastService = new MulticastService();
            this._multicastService.AnswerReceived += this.MulticastServiceOnAnswerReceived;
        }

        public override void Load()
        {
            this.LoadPluginIcons();
            this._multicastService.Start();
            this._refreshThread.Start();
        }

        public override void Unload()
        {
            this._multicastService.Dispose();
            this._httpClient.Dispose();
        }

        private void OnApplicationStarted(Object sender, EventArgs e)
        {
        }

        private void OnApplicationStopped(Object sender, EventArgs e)
        {
        }

        public override void RunCommand(String commandName, String parameter)
        {
        }

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff)
        {
        }
        
        private void LoadPluginIcons()
        {
            //var resources = this.Assembly.GetManifestResourceNames();
            this.Info.Icon16x16 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-16.png");
            this.Info.Icon32x32 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-32.png");
            this.Info.Icon48x48 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-48.png");
            this.Info.Icon256x256 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-256.png");
        }

        private void MulticastServiceOnAnswerReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.Answers.Any(a => a.CanonicalName == "_elg._tcp.local"))
            {
                var address = e.RemoteEndPoint.Address.ToString();
                var dnsName = e.Message.AdditionalRecords?
                    .First(resourceRecord => resourceRecord.Type == DnsType.TXT)
                    .Name
                    .Labels
                    .First();

                if (string.IsNullOrWhiteSpace(dnsName))
                    return;
                
                this.KeyLights.AddOrUpdate(dnsName, 
                    key => new KeyLightState
                    {
                        Address = address,
                        Id = dnsName,
                        Name = dnsName
                    },
                    (key, oldValue) => new KeyLightState
                    {
                        Address = address,
                        Id = oldValue.Id,
                        Name = oldValue.Name,
                        State = oldValue.State
                    });
                
                //AdditionalRecords:
                //A: IPv4 address record
                //AAAA: IPv6 address record
                //SRV: Service locator
                //TXT: Text record
                //NSEC (A, AAAA): Next Secure record
                //NSEC (TXT, SRV): Next Secure record
            }
        }

        private void RefreshKeyLightInfo()
        {
            var toggleCommand = base.DynamicCommands
                .OfType<ToggleCommand>()
                .Single();

            //var brightnessAdjustment = base.DynamicAdjustments
            //    .OfType<BrightnessAdjustment>()
            //    .Single();

            //var temperatureAdjustment = base.DynamicAdjustments
            //    .OfType<TemperatureAdjustment>()
            //    .Single();

            //Control Center refreshes lights, and accessory info each second (unordered, as in parallel?) if it's opened.
            while (true)
            {
                try
                {
                    foreach (var keyLightState in this.KeyLights)
                    {
                        var value = keyLightState.Value;
                        var infoRequestTask = this.KeyLightClient.GetAccessoryInfo(value.Address, CancellationToken.None);
                        var lightsResponseTask = this.KeyLightClient.GetLights(value.Address, CancellationToken.None);

                        Task.WhenAll(infoRequestTask, lightsResponseTask)
                            .GetAwaiter()
                            .GetResult();

                        var info = infoRequestTask.Result;
                        var light = lightsResponseTask.Result.Lights.First();

                        var state = keyLightState.Value;
                        state.Name = info.DisplayName;
                        state.State = light;

                        toggleCommand.SetState(state, light.On);
                        //brightnessAdjustment.SetBrightness(light.Brightness);
                        //temperatureAdjustment.SetTemperature(light.Temperature);
                    }
                }
                catch (Exception exception)
                {
                    Trace.TraceError($"Exception on RefreshThread: {exception}");
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }

    class KeyLightState
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public KeyLightLightModel State { get; set; }
    }
}
