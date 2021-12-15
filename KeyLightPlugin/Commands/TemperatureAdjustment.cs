namespace Loupedeck.KeyLightPlugin.Commands
{
    using System;
    using System.Threading;

    using Helpers;

    class TemperatureAdjustment : PluginDynamicAdjustment
    {
        private Int32 _temperature;
        private KeyLightPlugin _plugin;

        public TemperatureAdjustment()
            : base("Adjust temperature", "Adjusts the temperature", "KeyLight", false)
        {
            //
        }

        internal void SetTemperature(Int32 temperature)
        {
            //Current: 278
            //Control Center: Min 7000K (143), Max 2900K (344)
            this._temperature = RangeHelper.Range(temperature, 143, 344);

            this.AdjustmentValueChanged();
        }

        protected override Boolean OnLoad()
        {
            this._plugin = (KeyLightPlugin)base.Plugin;
            return true;
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 ticks)
        {
            var value = this._temperature + ticks;

            //TODO: What happens if exceptions etc.?
            this.SetTemperature(value);

            var keyLightClient = this._plugin.KeyLightClient;
            keyLightClient.SetTemperature(value, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
        
        protected override String GetAdjustmentValue(String actionParameter) =>
            $"{this.ToKelvin(this._temperature)}K";

        private Int32 ToKelvin(Int32 x)
        {
            //Approximation (off by ~ 50-150K):
            var ticks = 0.0900835 * x * x - 63.5864 * x + 14177.3;

            return (Int32)(Math.Round(ticks / 50.0) * 50.0);
        }
    }
}
