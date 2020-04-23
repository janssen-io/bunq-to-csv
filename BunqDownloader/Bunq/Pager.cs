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
        private readonly PageOrder order;

        public Pager(): this(50, PageOrder.Ascending) { }

        public Pager(int pageSize, PageOrder order)
        {
            this.pageSize = pageSize;
            this.order = order;
        }

        public IEnumerable<T> Read<T>(Func<IDictionary<string, string>, BunqResponse<List<T>>> ListT)
        {
            var pagination = new Pagination { Count = this.pageSize };
            var batch = ListT(pagination.UrlParamsCountOnly);

            while(batch.Value.Any())
            {
                foreach (var resource in batch.Value)
                    yield return resource;

                if (this.order == PageOrder.Ascending)
                {
                    if (batch.Pagination.NewerId is null)
                        yield break;

                    batch = ListT(batch.Pagination.UrlParamsNextPage);
                }
                else
                {
                    if (batch.Pagination.OlderId is null)
                        yield break;

                    batch = ListT(batch.Pagination.UrlParamsPreviousPage);
                }
            }
        }
    }

    public enum PageOrder
    {
        Ascending,
        Descending
    }
}
