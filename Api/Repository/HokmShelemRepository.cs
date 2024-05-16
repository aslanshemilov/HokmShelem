namespace Api.Repository
{
    public class HokmShelemRepository : IHokmShelemRepository
    {
        private readonly Context _context;
        private readonly ContextVisitors _hSContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly HttpClient _ip2locationHttpClient;

        public HokmShelemRepository(Context context,
            ContextVisitors hSContext,
            IConfiguration config,
            IMapper mapper)
        {
            _context = context;
            _hSContext = hSContext;
            _config = config;
            _mapper = mapper;
            _ip2locationHttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.ip2location.io")
            };
        }

        public async Task<IEnumerable<VisitorMessageDto>> GetAllVisitorMessagesAsync()
        {
            return _mapper.Map<IEnumerable<VisitorMessageDto>>(await _hSContext.VisitorMessage.Include(x => x.Visitor).ToListAsync());
        }

        public async Task<IEnumerable<VisitorDto>> GetAllVisitorsAsync()
        {
            return _mapper.Map<IEnumerable<VisitorDto>>(await _hSContext.Visitor.ToListAsync());
        }

        public async Task<Visitor> GetVisitorByIdAsync(int id)
        {
            var visitor = await _hSContext.Visitor.FindAsync(id);
            return visitor == null ? null : visitor;
        }

        public async Task<VisitorDto> GetVisitorDtoByIdAsync(int id)
        {
            var visitor = await _hSContext.Visitor.Include(c => c.Messsages).FirstOrDefaultAsync(x => x.Id == id);
            if (visitor == null) return null;

            var toReturn = _mapper.Map<VisitorDto>(visitor);
            toReturn.Messages = _mapper.Map<List<MessageAddDto>>(visitor.Messsages);
            return toReturn;
        }

        public async Task HandleVisitorAsync(string visitorIpAddress)
        {
            var visitor = await GetVisitorByIpAddressAsync(visitorIpAddress);

            if (visitor == null)
            {
                var ip2LocationResult = await GetIP2LocationResultAsync(visitorIpAddress);

                var newVisitorToAdd = new Visitor
                {
                    IpAddress = visitorIpAddress,
                    Country = ip2LocationResult.Country_Name,
                    City = ip2LocationResult.City_Name,
                    PostalCode = ip2LocationResult.Zip_Code,
                    Is_Proxy = ip2LocationResult.Is_Proxy,
                };

                _hSContext.Visitor.Add(newVisitorToAdd);
            }
            else
            {
                if (DateTime.UtcNow.Date > visitor.LastVisit.Date)
                {
                    visitor.LastVisit = DateTime.UtcNow;
                    visitor.NumberOfVisit++;
                }
            }
        }

        public void DeleteVisitor(Visitor visitor)
        {
            _hSContext.Visitor.Remove(visitor);
        }

        public async Task AddMessageAsync(string visitorIpAddress, MessageAddDto model)
        {
            var visitor = await GetVisitorByIpAddressAsync(visitorIpAddress);
            visitor.Messsages.Add(_mapper.Map<VisitorMessage>(model));
        }

        #region Private Mehtods
        private async Task<Visitor> GetVisitorByIpAddressAsync(string visitorIpAddress)
        {
            return await _hSContext.Visitor.FirstOrDefaultAsync(x => x.IpAddress.Equals(visitorIpAddress));
        }
        private async Task<IP2locationResultDto> GetIP2LocationResultAsync(string ip)
        {
            // https://www.ip2location.io/dashboard
            // username admin@hokmshelem.com

            try
            {
                var result = await _ip2locationHttpClient.GetFromJsonAsync<IP2locationResultDto>($"?Key={_config["IP2LocationKey"]}&ip={ip}&format=json");
                return result;
            }
            catch (Exception)
            {
                return new IP2locationResultDto();
            }
        }
        #endregion
    }
}
