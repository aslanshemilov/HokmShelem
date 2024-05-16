namespace Engine.Controllers
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

        [HttpGet("client-url")]
        public ActionResult<string> GetClientUrl()
        {
            return _config["JWT:ClientUrl"];
        }

        [HttpGet("issuer")]
        public ActionResult<string> GetIssuer()
        {
            return _config["JWT:Issuer"];
        }

        [HttpGet("engine-ready")]
        public ActionResult<string> EnginReady()
        {
            return "Engine is ready";
        }
    }
}
