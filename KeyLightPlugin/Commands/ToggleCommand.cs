namespace Loupedeck.KeyLightPlugin.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class ToggleCommand : PluginDynamicCommand
    {
        private LightState _lightState = LightState.Unknown;
        private KeyLightPlugin _plugin;

        public ToggleCommand()
            : base("Toggle light", "Toggle light On or Off", "KeyLight")
        {
            //
        }

        internal void SetState(LightState lightState)
        {
            this._lightState = lightState;

            this.ActionImageChanged();
        }

        protected override Boolean OnLoad()
        {
            this._plugin = (KeyLightPlugin)base.Plugin;

            return true;
        }
        
        protected override void RunCommand(String actionParameter)
        {
            var lightState = this._lightState == LightState.On
                ? LightState.Off
                : LightState.On;

            this.SetState(lightState);

            //TODO: What happens if exceptions etc.?
            var keyLightClient = this._plugin.KeyLightClient;
            keyLightClient.SetLightStatus(lightState, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Light {this._lightState}";
    }
}
