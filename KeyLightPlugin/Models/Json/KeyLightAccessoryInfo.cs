namespace Loupedeck.KeyLightPlugin.Models.Json
{
    public class KeyLightAccessoryInfo
    {
        public string ProductName { get; set; }
        public int HardwareBoardType { get; set; }
        public int FirmwareBuildNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public string SerialNumber { get; set; }
        public string DisplayName { get; set; }
        public string[] Features { get; set; }
    }
}