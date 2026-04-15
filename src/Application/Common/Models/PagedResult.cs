namespace Complex.Application.Common.Models
{
	public sealed class PagedResult<T>
	{
		public int Page { get; set; }
		public int PageSize { get; set; }

		public long Total { get; set; }

		public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

		public PagedResult() { }

		public PagedResult(List<T> items, long total, int page, int pageSize)
		{
			Page = page;
			PageSize = pageSize;
			Total = total;
			Items = items;
		}
	}
}
