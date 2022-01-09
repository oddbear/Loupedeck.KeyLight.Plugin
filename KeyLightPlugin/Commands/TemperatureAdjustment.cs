﻿using System;
using System.Linq;
using System.Threading;
using Loupedeck.KeyLightPlugin.Helpers;
using Loupedeck.KeyLightPlugin.Services;

namespace Loupedeck.KeyLightPlugin.Commands
{
    class TemperatureAdjustment : PluginDynamicAdjustment
    {
        private KeyLightPlugin _plugin;
        private KeyLightService _keyLightService;

        public TemperatureAdjustment()
            : base(false)
        {
            base.DisplayName = "Adjust temperature";
            base.GroupName = "";
            base.Description = "Adjusts the temperature";

            base.MakeProfileAction("list;Select KeyLight:");
        }
        
        protected override bool OnLoad()
        {
            _plugin = base.Plugin as KeyLightPlugin;
            if (_plugin is null)
                return false;

            _keyLightService = _plugin.KeyLightService;
            if (_keyLightService is null)
                return false;

            _plugin.TemperatureUpdatedEvents += (sender, e) => base.ActionImageChanged(e.Id);

            return true;
        }

        protected override void ApplyAdjustment(string actionParameter, int ticks)
        {
            var (address, light) = _plugin.GetKeyLight(actionParameter);
            if (address is null || light is null)
                return;

            try
            {
                if (light.Temperature is null)
                    return;

                //Current: 278
                //Control Center: Min 7000K (143), Max 2900K (344)
                var temperature = RangeHelper.Range(light.Temperature.Value + ticks, 143, 344);
                light.Temperature = temperature;
                
                base.ActionImageChanged(actionParameter);

                var cancellationToken = CancellationToken.None;
                _keyLightService.SetTemperature(address, temperature, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //TODO: Add proper logging
                throw;
            }
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            var (_, light) = _plugin.GetKeyLight(actionParameter);
            if (light is null)
                return null;
            
            var temperature = light.Temperature;

            return temperature is null
                ? null
                : $"{RangeHelper.ToKelvin(temperature.Value)}K";
        }

        protected override PluginActionParameter[] GetParameters() =>
            _plugin.KeyLights
                .Select(keyLight => new PluginActionParameter(keyLight.Key, keyLight.Value.Name, string.Empty))
                .ToArray();
    }
}
