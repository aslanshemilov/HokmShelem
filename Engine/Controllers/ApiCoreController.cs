namespace Engine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCoreController : ControllerBase
    {
        private IUnityRepo _unity;
        private IMapper _mapper;
        private IConfiguration _configuration;

        protected IUnityRepo Unity => _unity ??= HttpContext.RequestServices.GetService<IUnityRepo>();
        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
        protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>();
    }
}
