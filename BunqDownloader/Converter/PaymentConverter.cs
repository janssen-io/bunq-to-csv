using BunqDownloader.Bunq;
using System;
using System.IO;
using System.Linq;

namespace BunqDownloader.Converter
{
    class PaymentConverter
    {
        public static void Run(PagerConfiguration pagerConfig, CommandlineArguments parameters)
        {
            var today = DateTime.Today;
            var firstDayOfTheMonth = today.AddDays(-1 * today.Day + 1);

            var from = parameters.FromDate ?? pagerConfig.LastUpToDate ?? firstDayOfTheMonth;
            var to = today;

            var pager = new Pager(pageSize: pagerConfig.PageSize);
            var allPayments = pager.Read(from, to);

            using var writer = new StreamWriter(parameters.OutputPath);
            using var csvWriter = new TqlCsvWriter(writer);
            csvWriter.Write(allPayments.ToList());
        }
    }
}
