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
        private KeyLightPlugin _plugin;

        public ToggleCommand() : base("Toggle light", "Toggle light On or Off", "KeyLight")
        {
            //
        }

        protected override Boolean OnLoad()
        {
            this._plugin = (KeyLightPlugin)base.Plugin;
            return true;
        }
        
        protected override void RunCommand(String actionParameter)
        {
            var keyLightClient = this._plugin.KeyLightClient;
            keyLightClient.ToggleLight(CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }
    }
}
