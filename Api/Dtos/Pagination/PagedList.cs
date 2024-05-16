namespace Api.Dtos.Pagination
{
    public class PagedList<T> : List<T>
    {
        public PagedList()
        {

        }
        public PagedList(IEnumerable<T> items)
        {
            AddRange(items);
        }
        public PagedList(IEnumerable<T> items, int totalItemsCount, int pageNumber, int pageSize, int totalPages)
        {
            PageNumber = pageNumber;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalItemsCount = totalItemsCount;

            AddRange(items);
        }

        public int TotalItemsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var totalItemsCount = await source.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItemsCount / (double)pageSize);
            if (pageNumber > totalPages && totalPages > 0)
            {
                // set to last page
                pageNumber = totalPages;
            }

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, totalItemsCount, pageNumber, pageSize, totalPages);
        }
    }
}
