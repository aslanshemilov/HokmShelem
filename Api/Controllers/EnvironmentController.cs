namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EnvironmentController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult<string> GetEnvironment()
        {
            return _config["Environment"];
        }
    }
}
