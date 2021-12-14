﻿namespace Loupedeck.KeyLightPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    class KeyLightClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _serializerSettings;

        public KeyLightClient(HttpClient httpClient)
        {
            this._httpClient = httpClient;

            this._serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        public async Task SetBrightness(Int32 brightness, CancellationToken cancellationToken)
        {
            var lights = new KeyLightLightsModel
            {
                NumberOfLights = 1,
                Lights = new []
                {
                    new KeyLightLightModel { Brightness = brightness }
                }
            };
            var json = JsonConvert.SerializeObject(lights, this._serializerSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseMessage = await this._httpClient.PutAsync("http://192.168.50.102:9123/elgato/lights", content, cancellationToken);
        }

        public async Task ToggleLight(CancellationToken cancellationToken)
        {
            var lights = await this.GetLights(cancellationToken);

            foreach (var light in lights.Lights)
            {
                light.On = light.On == LightState.On
                    ? LightState.Off
                    : LightState.On;
            }

            var json = JsonConvert.SerializeObject(lights, this._serializerSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseMessage = await this._httpClient.PutAsync("http://192.168.50.102:9123/elgato/lights", content, cancellationToken);
        }

        public async Task<KeyLightLightsModel> GetLights(CancellationToken cancellationToken)
        {
            var responseMessage = await this._httpClient.GetAsync("http://192.168.50.102:9123/elgato/lights", cancellationToken);
            var json = await responseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<KeyLightLightsModel>(json);
        }

        public async Task<KeyLightAccessoryInfo> GetAccessoryInfo(CancellationToken cancellationToken)
        {
            var responseMessage = await this._httpClient.GetAsync("http://192.168.50.102:9123/elgato/accessory-info", cancellationToken);
            var json = await responseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<KeyLightAccessoryInfo>(json);
        }
    }
}
