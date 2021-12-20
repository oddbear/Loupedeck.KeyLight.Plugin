namespace Loupedeck.KeyLightPlugin.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.UI;

    class ToggleCommand : PluginDynamicCommand
    {
        private LightState _lightState = LightState.Unknown;
        private KeyLightPlugin _plugin;

        public ToggleCommand()
            : base()
        {
            base.DisplayName = "Toggle light";
            base.GroupName = "";
            base.Description = "Toggle light On or Off";

            base.MakeProfileAction("list;Select KeyLight:");
        }

        internal void SetState(KeyLightState state, LightState? lightState)
        {
            if (lightState is null)
                return;

            state.State.On = lightState;

            base.ActionImageChanged();
        }

        protected override Boolean OnLoad()
        {
            this._plugin = (KeyLightPlugin)base.Plugin;
            return true;
        }
        
        protected override void RunCommand(String actionParameter)
        {
            if(!this._plugin.KeyLights.TryGetValue(actionParameter, out var value))
                return;

            var lightState = value.State.On == LightState.On
                ? LightState.Off
                : LightState.On;

            this.SetState(value, lightState);

            //TODO: What happens if exceptions etc.?
            var keyLightClient = this._plugin.KeyLightClient;
            keyLightClient.SetLightStatus(value.Address, lightState, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        protected override PluginActionParameter[] GetParameters() =>
            this._plugin.KeyLights
                .Select(kv => kv.Value)
                .Select(keyLight => new PluginActionParameter(keyLight.Id, keyLight.Name, String.Empty))
                .ToArray();

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var text = "-";
                var state = LightState.Off;

                if (this._plugin.KeyLights.TryGetValue(actionParameter, out var value))
                {
                    state = value.State?.On ?? LightState.Off;
                    text = value.State.On?.ToString() ?? "-";
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
