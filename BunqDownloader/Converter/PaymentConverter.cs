using Bunq.Sdk.Http;
using Bunq.Sdk.Model.Generated.Endpoint;
using BunqDownloader.Bunq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace BunqDownloader.Converter
{
    class PaymentConverter
    {
        private readonly IChooseAccounts accountChooser;
        private readonly DateTime fromDate;
        private readonly DateTime upToAndIncludingDate;

        private readonly string outputPath;

        public PaymentConverter(PagerConfiguration pagerConfig, CommandlineArguments parameters, IChooseAccounts accountChooser)
        {
            this.accountChooser = accountChooser;
            var today = DateTime.Today;
            var firstDayOfTheMonth = today.AddDays(-1 * today.Day + 1);

            fromDate = (parameters.FromDate ?? pagerConfig.LastUpToDate ?? firstDayOfTheMonth).Date;
            upToAndIncludingDate = parameters.UpToDate.AddDays(-1).Date;

            if (fromDate > upToAndIncludingDate)
                throw new ArgumentException($"'From' cannot be past 'up to' date: {fromDate:yyyy/MM/dd} - {parameters.UpToDate:yyyy/MM/dd}");

            outputPath = parameters.OutputPath;
        }

        public void Run()
        {
            using var writer = string.IsNullOrEmpty(this.outputPath)
                ? new StreamWriter(Console.OpenStandardOutput())
                : new StreamWriter(this.outputPath);
            using var csvWriter = new TqlCsvWriter(writer);

            var accounts = ListAccounts();
            var chosenAccounts = accountChooser.Choose(accounts.ToArray());
            var paymentsInRange = ListPayments(chosenAccounts);
            csvWriter.Write(paymentsInRange);
        }

        private static IEnumerable<MonetaryAccountBank> ListAccounts()
        {
            var pager = new Pager(25, PageOrder.Descending);
            Func<IDictionary<string, string>, BunqResponse<List<MonetaryAccountBank>>> api =
                u => MonetaryAccountBank.List(urlParams: u);
            return pager.Read(api);
        }

        private IEnumerable<Payment> ListPayments(IEnumerable<MonetaryAccountBank> accounts)
        {
            var pager = new Pager(50, PageOrder.Descending);

            var paymentsInRange = Enumerable.Empty<Payment>();
            foreach (var account in accounts)
            {
                Func<IDictionary<string, string>, BunqResponse<List<Payment>>> api =
                    u => Payment.List(account.Id, urlParams: u);

                var paymentsForAccount = pager.Read(api);
                var paymentsAccountRange = this.FilterRange(paymentsForAccount);
                paymentsInRange = paymentsInRange.Concat(paymentsAccountRange);
            }

            return paymentsInRange;
        }

        private IEnumerable<Payment> FilterRange(IEnumerable<Payment> payments)
        {
            foreach (var payment in payments)
            {
                var createdAt = DateTime.Parse(payment.Created).Date;
                if (createdAt < this.fromDate)
                {
                    yield break;
                }

                if (createdAt <= this.upToAndIncludingDate)
                {
                    yield return payment;
                }
            }
        }
    }
}
