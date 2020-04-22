using System;

namespace BunqDownloader
{
    public class PagerConfiguration
    {
        public static readonly PagerConfiguration Default = new PagerConfiguration(20, null);

        public PagerConfiguration(int pageSize, DateTime? lastUpToDate)
        {
            if (pageSize < 1)
                throw new ArgumentException("Page size must be larger than 0.", nameof(pageSize));

            if (lastUpToDate != null && lastUpToDate > DateTime.Today)
                throw new ArgumentException("Last 'Up to' date cannot be in the future. ", nameof(lastUpToDate));

            this.PageSize = pageSize;
            this.LastUpToDate = lastUpToDate;
        }

        public int PageSize { get; }

        public DateTime? LastUpToDate { get; }

        public PagerConfiguration WithPageSize(int size) =>
            new PagerConfiguration(size, this.LastUpToDate);

        public PagerConfiguration WithLastUpToDate(DateTime? lastUpTo) =>
            new PagerConfiguration(this.PageSize, lastUpTo);
    }
}
