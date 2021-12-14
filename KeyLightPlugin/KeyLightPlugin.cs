namespace Loupedeck.KeyLightPlugin
{
    using System;
    using System.Net.Http;

    public class KeyLightPlugin : Plugin
    {
        private HttpClient _httpClient;
        internal KeyLightClient KeyLightClient { get; private set;  }

        public override void Load()
        {
            //TODO: Brightness adjustment
            //TODO: Warmth Adjustment
            this._httpClient = new HttpClient();
            this.KeyLightClient = new KeyLightClient(this._httpClient);
        }

        public override void Unload()
        {
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
