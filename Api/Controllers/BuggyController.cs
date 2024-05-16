namespace Api.Controllers
{
    public class BuggyController : ApiCoreController
    {
        [HttpGet("404-not-found")]
        public ActionResult Get404NotFoundRequest()
        {
            var userProfile = Context.UserProfile.Find(-1);
            if (userProfile == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok();
        }

        [HttpGet("400-bad-request")]
        public ActionResult GetBadrequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("400-validation-bad-request/{id}")]
        public ActionResult GetValidationBadrequest(int id)
        {
            return Ok();
        }

        [HttpPost("400-validation-bad-request-with-object")]
        public ActionResult GetValidationBadrequest(RegisterDto model)
        {
            return Ok();
        }

        [HttpGet("500-server-error")]
        public ActionResult Get500ServerError()
        {
            var userProfile = Context.UserProfile.Find(-1);
            var toReturn = userProfile.ToString();

            return Ok();
        }
    }
}
