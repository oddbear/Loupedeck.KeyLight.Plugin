using Loupedeck.KeyLightPlugin.Models.Enums;

namespace Loupedeck.KeyLightPlugin.Models.Events
{
    public class StateUpdatedEventArgs : LightUpdatedEventArgs
    {
        public LightState State { get; }

        public StateUpdatedEventArgs(string id, LightState state)
            : base(id)
        {
            State = state;
        }
    }
}