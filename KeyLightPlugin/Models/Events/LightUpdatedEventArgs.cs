using System;

namespace Loupedeck.KeyLightPlugin.Models.Events
{
    public abstract class LightUpdatedEventArgs : EventArgs
    {
        public string Id { get; }

        protected LightUpdatedEventArgs(string id)
        {
            Id = id;
        }
    }
}
