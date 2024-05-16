namespace Api.Dtos.Pagination
{
    public class PaginatedResult<T> where T : class
    {
        public PaginatedResult(int totalItemsCount, int pageNumber, int pageSize, int totalPages, IReadOnlyList<T> items)
        {
            TotalItemsCount = totalItemsCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
            Items = items;
        }

        public int TotalItemsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<T> Items { get; set; }
    }
}
