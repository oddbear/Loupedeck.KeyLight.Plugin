using System.Net.Http;
using System.Text;
using System.Threading;
using Loupedeck.KeyLightPlugin.Helpers;
using Loupedeck.KeyLightPlugin.Models.Enums;
using Loupedeck.KeyLightPlugin.Models.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Loupedeck.KeyLightPlugin.Services
{
    public class KeyLightService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _serializerSettings;

        public KeyLightService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
        
        internal void SetTemperature(string address, int temperature, CancellationToken cancellationToken)
        {
            //Control Center: Min 7000K (143), Max 2900K (344)
            temperature = RangeHelper.Range(temperature, 143, 344);

            var lights = new KeyLightLightsModel
            {
                NumberOfLights = 1,
                Lights = new[]
                {
                    new KeyLightLightModel { Temperature = temperature }
                }
            };

            var json = JsonConvert.SerializeObject(lights, _serializerSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = _httpClient.PutAsync($"http://{address}:9123/elgato/lights", content, cancellationToken)
                .GetAwaiter()
                .GetResult();
        }
        
        internal void SetBrightness(string address, int brightness, CancellationToken cancellationToken)
        {
            //Control Center: Min 3, Max 100
            brightness = RangeHelper.Range(brightness, 3, 100);

            var lights = new KeyLightLightsModel
            {
                NumberOfLights = 1,
                Lights = new []
                {
                    new KeyLightLightModel { Brightness = brightness }
                }
            };

            var json = JsonConvert.SerializeObject(lights, _serializerSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = _httpClient.PutAsync($"http://{address}:9123/elgato/lights", content, cancellationToken)
                .GetAwaiter()
                .GetResult();
        }
        
        internal void SetLightStatus(string address, LightState lightState, CancellationToken cancellationToken)
        {
            if (lightState != LightState.On && lightState != LightState.Off)
                lightState = LightState.Off;

            var lights = new KeyLightLightsModel
            {
                NumberOfLights = 1,
                Lights = new[]
                {
                    new KeyLightLightModel { On = lightState }
                }
            };
            
            var json = JsonConvert.SerializeObject(lights, _serializerSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = _httpClient.PutAsync($"http://{address}:9123/elgato/lights", content, cancellationToken)
                .GetAwaiter()
                .GetResult();
        }
        
        internal KeyLightLightsModel GetLights(string address, CancellationToken cancellationToken)
        {
            var responseMessage = _httpClient.GetAsync($"http://{address}:9123/elgato/lights", cancellationToken)
                .GetAwaiter()
                .GetResult();

            var json = responseMessage.Content.ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            return JsonConvert.DeserializeObject<KeyLightLightsModel>(json);
        }
        
        internal KeyLightSettings GetLightsSettings(string address, CancellationToken cancellationToken)
        {
            var responseMessage = _httpClient.GetAsync($"http://{address}:9123/elgato/lights/settings", cancellationToken)
                .GetAwaiter()
                .GetResult();

            var json = responseMessage.Content.ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            return JsonConvert.DeserializeObject<KeyLightSettings>(json);
        }
        
        internal KeyLightAccessoryInfo GetAccessoryInfo(string address, CancellationToken cancellationToken)
        {
            var responseMessage = _httpClient.GetAsync($"http://{address}:9123/elgato/accessory-info", cancellationToken)
                .GetAwaiter()
                .GetResult();

            var json = responseMessage.Content.ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            return JsonConvert.DeserializeObject<KeyLightAccessoryInfo>(json);
        }
    }
}
