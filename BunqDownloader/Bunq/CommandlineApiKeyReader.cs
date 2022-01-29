using System;

namespace BunqDownloader.Bunq
{
    class CommandlineApiKeyReader : IApiKeyReader
    {
        public string Read()
        {
            Console.Write("API key: ");
            var key = Console.ReadLine();
            if (key == null)
            {
                Environment.Exit(0);
            }
            
            return key.Trim();
        }
    }
}
