namespace Api.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly Context _context;

        public CountryRepository(Context context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Country>> GetAllAsync()
        {
            var countryList = new List<Country>()
            {
                new Country()
                {
                    Id = 0,
                    Name = "Please choose your country"
                }
            };
            countryList.AddRange(await _context.Country.ToListAsync());

            return countryList;
        }
        public async Task<Country> GetCountryByIdAsync(int id)
        {
            return await _context.Country.FindAsync(id);
        }
        public async Task<Country> GetCountryByNameAsync(string countryName)
        {
            return await _context.Country.FirstOrDefaultAsync(x => x.Name == countryName);
        }

        public async Task<int> GetCountryIdByNameAsync(string countryName)
        {
            return await _context.Country.Where(x => x.Name == countryName).Select(x => x.Id).FirstOrDefaultAsync();
        }
    }
}


