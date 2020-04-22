using Bunq.Sdk.Http;
using Bunq.Sdk.Model.Generated.Endpoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BunqDownloader.Bunq
{
    public class Pager
    {
        private readonly int pageSize;
        private DateTime fromDate;
        private DateTime upToAndIncludingDate;

        public Pager(): this(50) { }

        public Pager(int pageSize)
        {
            this.pageSize = pageSize;
        }

        public IEnumerable<Payment> Read(DateTime fromDate, DateTime upToDate)
        {
            if (fromDate >= upToDate)
            {
                throw new ArgumentException($"Invalid date range: {fromDate} to {upToDate}");
            }

            this.fromDate = fromDate;
            this.upToAndIncludingDate = upToDate.AddDays(-1);

            foreach (var payment in PagePayments())
            {
                var createdAt = DateTime.Parse(payment.Created);
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
        private IEnumerable<Payment> PagePayments()
        {
            var pagination = new Pagination { Count = this.pageSize };
            var batch = Payment.List(urlParams: pagination.UrlParamsCountOnly);

            while(batch.Value.Any())
            {
                foreach (var payment in batch.Value)
                    yield return payment;

                batch = Payment.List(urlParams: batch.Pagination.UrlParamsPreviousPage);
            }
        }
    }
}
