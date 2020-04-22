using Bunq.Sdk.Model.Generated.Endpoint;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BunqDownloader.Converter
{
    public sealed class TqlCsvWriter : IDisposable
    {
        private readonly TextWriter textwriter;
        private readonly CsvWriter writer;

        public TqlCsvWriter(TextWriter writer)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            this.textwriter = writer;
            this.writer = new CsvWriter(writer, config);
        }

        public void Dispose() => this.writer.Dispose();

        public void Write(IEnumerable<Payment> payments)
        {
            this.writer.WriteHeader(typeof(BunqRow));
            this.writer.Flush();
            this.textwriter.WriteLine();

            var rows = payments.Select(MapPaymentToRow);
            this.writer.WriteRecords(rows);
        }

        private BunqRow MapPaymentToRow(Payment payment) =>
            new BunqRow
            {
                CreatedAt = DateTime.Parse(payment.Created).ToString("yyyy/MM/dd"),
                Amount = payment.Amount.Value,
                CounterParty = payment.CounterpartyAlias.LabelMonetaryAccount.Iban,
                CounterPartyName = payment.CounterpartyAlias.LabelMonetaryAccount.DisplayName,
                Account = payment.Alias.LabelMonetaryAccount.Iban,
                AccountName = payment.Alias.LabelMonetaryAccount.DisplayName,
                Description = payment.Description.Trim(),
                Currency = payment.Amount.Currency
            };
    }
}
