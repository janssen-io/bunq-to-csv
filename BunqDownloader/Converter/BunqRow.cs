using System;

namespace BunqDownloader.Converter
{
    public class BunqRow
    {
        public string CreatedAt { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string CounterParty { get; set; }
        public string CounterPartyName { get; set; }
        public string Account { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
    }
}
