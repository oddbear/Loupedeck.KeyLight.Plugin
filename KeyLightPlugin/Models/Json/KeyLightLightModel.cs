using Loupedeck.KeyLightPlugin.Models.Enums;

namespace Loupedeck.KeyLightPlugin.Models.Json
{
    public class KeyLightLightModel
    {
        public LightState? On { get; set; }
        public int? Brightness { get; set; }
        public int? Temperature { get; set; }
    }
}