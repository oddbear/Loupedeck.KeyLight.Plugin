namespace Loupedeck.KeyLightPlugin
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;

    using Commands;

    public class KeyLightPlugin : Plugin
    {
        private readonly HttpClient _httpClient;
        internal KeyLightClient KeyLightClient { get; }
        private readonly Thread _refreshThread;

        public KeyLightPlugin()
        {
            this._httpClient = new HttpClient();
            this.KeyLightClient = new KeyLightClient(this._httpClient);
            this._refreshThread = new Thread(this.RefreshKeyLightInfo) { IsBackground = true };
        }
        
        private void RefreshKeyLightInfo()
        {
            var toggleCommand = base.DynamicCommands
                .OfType<ToggleCommand>()
                .Single();

            var brightnessAdjustment = base.DynamicAdjustments
                .OfType<BrightnessAdjustment>()
                .Single();

            var temperatureAdjustment = base.DynamicAdjustments
                .OfType<TemperatureAdjustment>()
                .Single();

            //Control Center refreshes lights, and accessory info each second (unordered, as in parallel?) if it's opened.
            while (true)
            {
                try
                {
                    //Loaded after BrightnessAdjustment, need to init in constructor.
                    var lights = this.KeyLightClient
                        .GetLights(CancellationToken.None)
                        .GetAwaiter()
                        .GetResult();

                    var lightState = lights.Lights
                        .FirstOrDefault()
                        ?.On ?? LightState.Unknown;
                    
                    var brightness = lights.Lights
                        .FirstOrDefault()
                        ?.Brightness ?? 0;

                    var temperature = lights.Lights
                        .FirstOrDefault()
                        ?.Temperature ?? 0;

                    toggleCommand.SetState(lightState);

                    brightnessAdjustment.SetBrightness(brightness);

                    temperatureAdjustment.SetTemperature(temperature);
                }
                catch (Exception exception)
                {
                    Trace.TraceError($"Exception on RefreshThread: {exception}");
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        public override void Load() =>
            this._refreshThread.Start();

        public override void Unload() =>
            this._httpClient.Dispose();

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

        private String[] GetKeyLights()
        {
            //TODO: Discovery or get from Control Center etc.?
            return new[]
            {
                "192.168.50.56"
            };
        }
    }
}
