namespace Api.Dtos.Admin
{
    public class A_MemberParams : BaseParams
    {
        public string CurrentUsername { get; set; }

        private string _sortBy;
        public string SortBy
        {
            get => _sortBy;
            set => _sortBy = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }

        private string _search;
        public string Search
        {
            get => _search;
            set => _search = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }

        private string _roles;
        public string Role
        {
            get => _roles;
            set => _roles = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => _status = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }

        private string _provider;
        public string Provider
        {
            get => _provider;
            set => _provider = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }
    }
}
