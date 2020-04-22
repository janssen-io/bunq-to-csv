using System;
using System.ComponentModel;

namespace BunqDownloader
{
    class CommandlineArguments
    {
        [Description("--fromDate:\t\tSet the date from where the transactions must be read (inclusive).")]
        public DateTime? FromDate { get; set; }

        [Description("--upToDate:\t\tSet the date up to where the transactions must be read (exclusive).")]
        public DateTime UpToDate { get; set; } = DateTime.Today;

        [Description("--outputPath:\t\tSet the location of the csv.")]
        public string OutputPath { get; set; } = $"bunq-{DateTime.Today:yyyyMMdd}.csv";

        [Description("--configDirectory:\tSet the location of bunq and pager configuration.")]
        public string ConfigDirectory { get; set; } = "./";

        [Description("--sandbox:\t\tUse the Bunq Sandbox API environment.")]
        public bool Sandbox { get; set; } = false;

        [Description("--debug:\t\tPrint stacktraces")]
        public bool Debug { get; set; } = false;

        public static void PrintHelp()
        {
            var defaultParams = new CommandlineArguments();
            var properties = typeof(CommandlineArguments).GetProperties();
            foreach (var property in properties)
            {
                var attr = (DescriptionAttribute)
                    property.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];

                var value = property.GetValue(defaultParams);
                Console.WriteLine($"{attr.Description} (default: {value ?? "null"})");
            }
        }
    }
}
