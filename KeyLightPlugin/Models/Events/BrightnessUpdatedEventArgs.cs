namespace Loupedeck.KeyLightPlugin.Models.Events
{
    public class BrightnessUpdatedEventArgs : LightUpdatedEventArgs
    {
        public int Brightness { get; set; }

        public BrightnessUpdatedEventArgs(string id, int brightness)
            : base(id)
        {
            Brightness = brightness;
        }
    }
}