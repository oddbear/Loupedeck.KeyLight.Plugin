namespace Loupedeck.KeyLightPlugin.Models.Events
{
    public class TemperatureUpdatedEventArgs : LightUpdatedEventArgs
    {
        public int Temperature { get; set; }

        public TemperatureUpdatedEventArgs(string id, int temperature)
            : base(id)
        {
            Temperature = temperature;
        }
    }
}