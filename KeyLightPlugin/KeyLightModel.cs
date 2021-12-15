namespace Loupedeck.KeyLightPlugin
{
    using System;

    class KeyLightLightsModel
    {
        public Int32 NumberOfLights { get; set; }
        public KeyLightLightModel[] Lights { get; set; }
    }

    class KeyLightLightModel
    {
        public LightState? On { get; set; }
        public Int32? Brightness { get; set; }
        public Int32? Temperature { get; set; }
    }

    class KeyLightAccessoryInfo
    {
        public String ProductName { get; set; }
        public Int32 HardwareBoardType { get; set; }
        public Int32 FirmwareBuildNumber { get; set; }
        public String FirmwareVersion { get; set; }
        public String SerialNumber { get; set; }
        public String DisplayName { get; set; }
        public String[] Features { get; set; }
    }

    class KeyLightSettings
    {
        public Int32 PowerOnBehavior { get; set; }
        public Int32 PowerOnBrightness { get; set; }
        public Int32 PowerOnTemperature { get; set; }
        public Int32 SwitchOnDurationMs { get; set; }
        public Int32 SwitchOffDurationMs { get; set; }
        public Int32 ColorChangeDurationMs { get; set; }
    }

    enum LightState
    {
        Off = 0,
        On = 1,
        Unknown = 2
    }
}
