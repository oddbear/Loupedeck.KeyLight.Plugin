using System;
using System.Linq;
using System.Threading;
using Loupedeck.KeyLightPlugin.Models.Enums;
using Loupedeck.KeyLightPlugin.Services;

namespace Loupedeck.KeyLightPlugin.Commands
{
    class ToggleCommand : PluginDynamicCommand
    {
        private KeyLightPlugin _plugin;
        private KeyLightService _keyLightService;

        public ToggleCommand()
            : base()
        {
            base.DisplayName = "Toggle light";
            base.GroupName = "";
            base.Description = "Toggle light On or Off";

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

            _plugin.StateUpdatedEvents += (sender, e) => base.ActionImageChanged(e.Id);

            return true;
        }
        
        protected override void RunCommand(string actionParameter)
        {
            var (address, light) = _plugin.GetKeyLight(actionParameter);
            if (address is null || light is null)
                return;

            try
            {
                var lightState = light.On == LightState.On
                    ? LightState.Off
                    : LightState.On;

                var cancellationToken = CancellationToken.None;
                _keyLightService.SetLightStatus(address, lightState, cancellationToken);

                light.On = lightState;
                base.ActionImageChanged(actionParameter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //TODO: Add proper logging
            }
        }

        protected override PluginActionParameter[] GetParameters() =>
            _plugin.KeyLights
                .Select(keyLight => new PluginActionParameter(keyLight.Key, keyLight.Value.Name, string.Empty))
                .ToArray();

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            var (_, light) = _plugin.GetKeyLight(actionParameter);
            if (light is null)
                return null;

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var text = "-";
                var state = LightState.Off;

                if (_plugin.KeyLights.TryGetValue(actionParameter, out var value))
                {
                    state = light.On ?? LightState.Off;
                    text = light.On?.ToString() ?? "-";
                }

                if (imageSize == PluginImageSize.Width60)
                {
                    var path = $"Loupedeck.KeyLightPlugin.Resources.KeyLight.{state.ToString().ToLower()}-50.png";
                    var background = EmbeddedResources.ReadImage(path);
                    bitmapBuilder.SetBackgroundImage(background);

                    bitmapBuilder.Translate(0, -7);
                }
                else
                {
                    var path = $"Loupedeck.KeyLightPlugin.Resources.KeyLight.{state.ToString().ToLower()}-80.png";
                    var background = EmbeddedResources.ReadImage(path);
                    bitmapBuilder.SetBackgroundImage(background);

                    bitmapBuilder.Translate(0, -11);
                }

                bitmapBuilder.DrawText(text, BitmapColor.Black);

                return bitmapBuilder.ToImage();
            }
        }
    }
}
