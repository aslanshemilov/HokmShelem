namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCoreController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IHostEnvironment _environment;
        private IConfiguration _configuration;
        private Context _context;
        protected IUnitOfWork UnitOfWork => _unitOfWork ??= HttpContext.RequestServices.GetService<IUnitOfWork>();
        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
        protected IHostEnvironment Environment => _environment ??= HttpContext.RequestServices.GetService<IHostEnvironment>();
        protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected Context Context => _context ??= HttpContext.RequestServices.GetService<Context>();
    }
}
