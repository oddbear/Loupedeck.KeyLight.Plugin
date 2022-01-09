using System;
using System.Linq;
using System.Threading;
using Loupedeck.KeyLightPlugin.Helpers;
using Loupedeck.KeyLightPlugin.Services;


namespace Loupedeck.KeyLightPlugin.Commands
{
    class BrightnessAdjustment : PluginDynamicAdjustment
    {
        private KeyLightPlugin _plugin;
        private KeyLightService _keyLightService;

        public BrightnessAdjustment()
            : base(false)
        {
            base.DisplayName = "Adjust brightness";
            base.GroupName = "";
            base.Description = "Adjusts the brightness";

            base.MakeProfileAction("list;Select KeyLight:");
        }
        
        protected override bool OnLoad()
        {
            _plugin = base.Plugin as KeyLightPlugin;
            if (_plugin is null)
                return false;

            _keyLightService = _plugin.KeyLightService;
            if (_keyLightService is null)
                return false;
            
            _plugin.BrightnessUpdatedEvents += (sender, e) => base.ActionImageChanged(e.Id);

            return true;
        }

        protected override void ApplyAdjustment(string actionParameter, int ticks)
        {
            var (address, light) = _plugin.GetKeyLight(actionParameter);
            if (address is null || light is null)
                return;

            try
            {
                if (light.Brightness is null)
                    return;

                var brightness = RangeHelper.Range(light.Brightness.Value + ticks, 3, 100);
                light.Brightness = brightness;

                base.ActionImageChanged(actionParameter);

                var cancellationToken = CancellationToken.None;
                _keyLightService.SetBrightness(address, brightness, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //TODO: Add proper logging
            }
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            var (address, light) = _plugin.GetKeyLight(actionParameter);
            if (address is null || light is null)
                return null;

            return light.Brightness?.ToString();
        }

        protected override PluginActionParameter[] GetParameters() =>
            _plugin.KeyLights
                .Select(keyLight => new PluginActionParameter(keyLight.Key, keyLight.Value.Name, string.Empty))
                .ToArray();

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            var (address, light) = _plugin.GetKeyLight(actionParameter);
            if (address is null || light is null)
                return null;
            
            var brightness = light.Brightness;
            if (brightness is null)
                return null;

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness100-50.png";
                if (brightness <= 30)
                    path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness030-50.png";
                if (brightness <= 15)
                    path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness015-50.png";
                if (brightness <= 5)
                    path = "Loupedeck.KeyLightPlugin.Resources.KeyLight.brightness005-50.png";

                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);

                bitmapBuilder.Translate(0, 18);
                bitmapBuilder.DrawText("brightness", BitmapColor.White, 10);

                return bitmapBuilder.ToImage();
            }
        }
    }
}
