namespace Api.IRepository
{
    public interface ICountryRepository
    {
        Task<IReadOnlyList<Country>> GetAllAsync();
        Task<Country> GetCountryByIdAsync(int id);
        Task<Country> GetCountryByNameAsync(string countryName);
        Task<int> GetCountryIdByNameAsync(string countryName);
    }
}
