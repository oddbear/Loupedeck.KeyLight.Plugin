//namespace Loupedeck.KeyLightPlugin.Commands
//{
//    using System;
//    using System.Linq;
//    using System.Threading;

//    using Helpers;

//    class BrightnessAdjustment : PluginDynamicAdjustment
//    {
//        private Int32 _brightness;
//        private KeyLightPlugin _plugin;

//        public BrightnessAdjustment()
//            : base("Adjust brightness", "Adjusts the brightness", "KeyLight", true) //true -> false (5.0.2 issue)
//        {
//            //
//        }

//        internal void SetBrightness(Int32? brightness)
//        {
//            if(brightness is null)
//                return;

//            //Control Center: Min 3, Max 100
//            this._brightness = RangeHelper.Range(brightness.Value, 3, 100);

//            this.AdjustmentValueChanged();
//        }

//        protected override Boolean OnLoad()
//        {
//            this._plugin = (KeyLightPlugin)base.Plugin;
//            return true;
//        }
        
//        protected override void ApplyAdjustment(String actionParameter, Int32 ticks)
//        {
//            var value = this._brightness + ticks;

//            //TODO: What happens if exceptions etc.?
//            this.SetBrightness(value);

//            //var keyLightClient = this._plugin.KeyLightClient;
//            //keyLightClient.SetBrightness(value, CancellationToken.None)
//            //    .GetAwaiter()
//            //    .GetResult();
//        }
        
//        protected override String GetAdjustmentValue(String actionParameter) =>
//            this._brightness.ToString();

//        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
//        {
//            using (var bitmapBuilder = new BitmapBuilder(imageSize))
//            {
//                var path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness100-50.png";
//                if (this._brightness <= 30)
//                    path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness030-50.png";
//                if (this._brightness <= 15)
//                    path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness015-50.png";
//                if (this._brightness <= 5)
//                    path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness005-50.png";

//                var background = EmbeddedResources.ReadImage(path);
//                bitmapBuilder.SetBackgroundImage(background);

//                bitmapBuilder.Translate(0, 18);
//                bitmapBuilder.DrawText("brightness", BitmapColor.White, 10);

//                return bitmapBuilder.ToImage();
//            }
//        }
//    }
//}
