using System;

namespace BunqDownloader.Bunq
{
    class CommandlineApiKeyReader : IApiKeyReader
    {
        public string Read()
        {
            Console.Write("API key: ");
            return Console.ReadLine().Trim();
        }
    }
}
