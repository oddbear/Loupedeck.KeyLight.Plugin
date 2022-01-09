using Loupedeck.KeyLightPlugin.Models.Json;

namespace Loupedeck.KeyLightPlugin.Models
{
    public class DiscoveredKeyLight
    {
        public string Address { get; set; }

        public string Id { get; set; }

        public string Name => AccessoryInfo?.DisplayName ?? Id;
        
        public KeyLightAccessoryInfo AccessoryInfo { get; set; }

        public KeyLightLightsModel Lights { get; set; }

        public KeyLightSettings Settings { get; set; }
    }
}
