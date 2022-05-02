using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using Makaretu.Dns;
using Loupedeck.KeyLightPlugin.Services;
using Loupedeck.KeyLightPlugin.Models;
using Loupedeck.KeyLightPlugin.Models.Events;
using Loupedeck.KeyLightPlugin.Models.Json;

namespace Loupedeck.KeyLightPlugin
{
    public class KeyLightPlugin : Plugin
    {
        private readonly HttpClient _httpClient;
        private readonly MulticastService _multicastService;
        private readonly Thread _discoveryThread;

        public KeyLightService KeyLightService { get; }
        public Dictionary<string, DiscoveredKeyLight> KeyLights { get; } = new Dictionary<string, DiscoveredKeyLight>();

        public event EventHandler<StateUpdatedEventArgs> StateUpdatedEvents;
        public event EventHandler<BrightnessUpdatedEventArgs> BrightnessUpdatedEvents;
        public event EventHandler<TemperatureUpdatedEventArgs> TemperatureUpdatedEvents;

        public override bool HasNoApplication => true;
        public override bool UsesApplicationApiOnly => true;

        public KeyLightPlugin()
        {
            _httpClient = new HttpClient();
            KeyLightService = new KeyLightService(_httpClient);

            _multicastService = new MulticastService();
            _multicastService.AnswerReceived += MulticastServiceOnAnswerReceived;

            _discoveryThread = new Thread(DiscoveryThread) {IsBackground = true};
        }

        private void DiscoveryThread()
        {
            while (true)
            {
                try
                {
                    //Query once a second for new lights:
                    _multicastService.SendQuery("_elg._tcp.local.");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
                catch
                {
                    //
                }
            }
        }
        
        public override void Load()
        {
            LoadPluginIcons();
            _multicastService.Start();
            _discoveryThread.Start();
        }

        public override void Unload()
        {
            _multicastService.Dispose();
            _discoveryThread.Interrupt();
            _httpClient.Dispose();
        }
        
        private void MulticastServiceOnAnswerReceived(object sender, MessageEventArgs e)
        {
            try
            {
                if (e.Message.Answers.All(resourceRecord => resourceRecord.CanonicalName != "_elg._tcp.local"))
                    return;

                var address = e.RemoteEndPoint.Address.ToString();
                if (e.RemoteEndPoint.Address.AddressFamily == AddressFamily.InterNetworkV6)
                    address = $"[{address}]"; //[...%10]:1234, the [] is needed to be allowed to specify a port (IPv6 contains ':' chars), and '%' is to scope to a interface number.

                var dnsName = e.Message.AdditionalRecords?
                    .First(resourceRecord => resourceRecord.Type == DnsType.TXT)
                    .Name
                    .Labels
                    .First();

                if (string.IsNullOrWhiteSpace(dnsName))
                    return;
                
                //Light found, check if it's a new one or existing one:
                if (!KeyLights.TryGetValue(dnsName, out var keyLight))
                    KeyLights[dnsName] = keyLight = new DiscoveredKeyLight { Id = dnsName };
                
                //Updates address:
                keyLight.Address = address;

                //Updates states:
                var cancellationToken = CancellationToken.None;

                //Update lights state, and raise lights updated events:
                keyLight.Lights = KeyLightService.GetLights(address, cancellationToken);
                LightsUpdated(dnsName, keyLight.Lights);

                keyLight.AccessoryInfo = KeyLightService.GetAccessoryInfo(address, cancellationToken);
                keyLight.Settings = KeyLightService.GetLightsSettings(address, cancellationToken);
                
                //AdditionalRecords:
                //A: IPv4 address record
                //AAAA: IPv6 address record
                //SRV: Service locator
                //TXT: Text record
                //NSEC (A, AAAA): Next Secure record
                //NSEC (TXT, SRV): Next Secure record
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LightsUpdated(string id, KeyLightLightsModel lightsModel)
        {
            foreach (var light in lightsModel.Lights)
            {
                if (light.On != null)
                {
                    var stateUpdatedEventArgs = new StateUpdatedEventArgs(id, light.On.Value);
                    StateUpdatedEvents?.Invoke(this, stateUpdatedEventArgs);
                }

                if (light.Brightness != null)
                {
                    var brightnessUpdatedEventArgs = new BrightnessUpdatedEventArgs(id, light.Brightness.Value);
                    BrightnessUpdatedEvents?.Invoke(this, brightnessUpdatedEventArgs);
                }

                if (light.Temperature != null)
                {
                    var temperatureUpdatedEventArgs = new TemperatureUpdatedEventArgs(id, light.Temperature.Value);
                    TemperatureUpdatedEvents?.Invoke(this, temperatureUpdatedEventArgs);
                }
            }
        }

        public (string address, KeyLightLightModel keyLight) GetKeyLight(string actionParameter)
        {
            try
            {
                if (actionParameter is null)
                    return default;

                if (!KeyLights.TryGetValue(actionParameter, out var value))
                    return default;

                var light = KeyLights[actionParameter]
                    .Lights
                    .Lights
                    .First();

                return (value.Address, light);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }

        public override void RunCommand(string commandName, string parameter)
        {
        }

        public override void ApplyAdjustment(string adjustmentName, string parameter, int diff)
        {
        }

        private void LoadPluginIcons()
        {
            //var resources = this.Assembly.GetManifestResourceNames();
            Info.Icon16x16 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-16.png");
            Info.Icon32x32 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-32.png");
            Info.Icon48x48 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-48.png");
            Info.Icon256x256 = EmbeddedResources.ReadImage("Loupedeck.KeyLightPlugin.Resources.Icons.icon-256.png");
        }
    }
}
