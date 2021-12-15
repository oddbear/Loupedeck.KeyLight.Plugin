namespace Loupedeck.KeyLightPlugin.Commands
{
    using System;
    using System.Linq;
    using System.Threading;

    using Helpers;

    class BrightnessAdjustment : PluginDynamicAdjustment
    {
        private Int32 _brightness;
        private KeyLightPlugin _plugin;

        public BrightnessAdjustment()
            : base("Adjust brightness", "Adjusts the brightness", "KeyLight", false)
        {
            //
        }

        internal void SetBrightness(Int32 brightness)
        {
            //Control Center: Min 3, Max 100
            this._brightness = RangeHelper.Range(brightness, 3, 100);

            this.AdjustmentValueChanged();
        }

        protected override Boolean OnLoad()
        {
            this._plugin = (KeyLightPlugin)base.Plugin;
            return true;
        }
        
        protected override void ApplyAdjustment(String actionParameter, Int32 ticks)
        {
            var value = this._brightness + ticks;

            //TODO: What happens if exceptions etc.?
            this.SetBrightness(value);

            var keyLightClient = this._plugin.KeyLightClient;
            keyLightClient.SetBrightness(value, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
        
        protected override String GetAdjustmentValue(String actionParameter) =>
            this._brightness.ToString();
    }
}
