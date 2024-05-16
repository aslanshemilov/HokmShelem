namespace Api.Dtos.Pagination
{
    public class BaseParams
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize || value < 0 ? MaxPageSize : value;
        }
    }
}
