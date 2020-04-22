using Newtonsoft.Json;
using System.IO;

namespace BunqDownloader
{
    class PagerConfigurationLoader
    {
        private readonly string pagerConfigPath;

        public PagerConfigurationLoader(string configPath)
        {
            pagerConfigPath = configPath;
        }

        public PagerConfiguration Read()
        {
            if (!File.Exists(pagerConfigPath))
                return PagerConfiguration.Default;

            var serializedConfig = File.ReadAllText(pagerConfigPath);
            return JsonConvert.DeserializeObject<PagerConfiguration>(serializedConfig);
        }
        
        public void Write(PagerConfiguration config)
        {
            var serializedConfig = JsonConvert.SerializeObject(config);
            File.WriteAllText(pagerConfigPath, serializedConfig);
        }
    }
}
