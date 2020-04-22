using Bunq.Sdk.Context;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using System;
using System.IO;
using System.Threading;

namespace BunqDownloader.Bunq
{
    class Initializer
    {
        private readonly string configPath;
        private ApiEnvironmentType apiEnv = ApiEnvironmentType.SANDBOX;
        private readonly IApiKeyReader apiKeyReader;

        public Initializer(ApiEnvironmentType apiEnv, IApiKeyReader reader, string bunqConfigPath)
        {
            this.apiEnv = apiEnv;
            this.apiKeyReader = reader;
            this.configPath = bunqConfigPath;
        }

        public void InitializeApiContext()
        {
            if (!File.Exists(this.configPath))
            {
                CreateApiContext();
            }
            else
            {
                RestoreApiContext();
            }


            if (BunqContext.ApiContext.EnvironmentType == ApiEnvironmentType.SANDBOX)
            {
                AddSandboxPayments();
            }
        }

        private void CreateApiContext()
        {
            string apiKey = this.apiKeyReader.Read();
            var api = ApiContext.Create(apiEnv, apiKey, Environment.MachineName);
            api.Save(this.configPath);

            BunqContext.LoadApiContext(api);
        }

        private void RestoreApiContext()
        {
            var apiContext = ApiContext.Restore(this.configPath);
            apiContext.EnsureSessionActive();
            apiContext.Save(this.configPath);

            BunqContext.LoadApiContext(apiContext);
        }

        private void AddSandboxPayments()
        {
            RequestInquiry.Create(
                new Amount("500.00", "EUR"),
                new Pointer("EMAIL", "sugardaddy@bunq.com"),
                "Requesting some spending money.",
                false
            );

            Thread.Sleep(1000);
        }
    }
}
