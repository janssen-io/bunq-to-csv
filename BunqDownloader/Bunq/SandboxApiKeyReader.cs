using Bunq.Sdk.Context;
using Bunq.Sdk.Json;
using Bunq.Sdk.Model.Generated.Endpoint;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace BunqDownloader.Bunq
{
    class SandboxApiKeyReader : IApiKeyReader
    {
        public string Read()
        {
            var sandboxUser = GenerateNewSandboxUser();
            return sandboxUser.ApiKey;
        }

        private SandboxUser GenerateNewSandboxUser()
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Client-Request-Id", "unique");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Geolocation", "0 0 0 0 NL");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Language", "en_US");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Region", "en_US");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "hoi");

            var requestTask = httpClient.PostAsync(ApiEnvironmentType.SANDBOX.BaseUri + "sandbox-user", null);
            requestTask.Wait();

            var responseString = requestTask.Result.Content.ReadAsStringAsync().Result;
            var responseJson = BunqJsonConvert.DeserializeObject<JObject>(responseString);
            return BunqJsonConvert.DeserializeObject<SandboxUser>(responseJson.First.First.First.First.First
                .ToString());
        }
    }
}
