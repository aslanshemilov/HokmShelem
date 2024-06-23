using Mailjet.Client.Resources;
using System;

namespace Api.Controllers
{
    public class HokmShelemController : ApiCoreController
    {
        [HttpGet]
        public async Task<IActionResult> VisitorConnected()
        {
            if (Environment.IsDevelopment()) return Ok();

            var visitorIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            await UnitOfWork.HokmShelemRepository.HandleVisitorAsync(visitorIp);

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        [HttpPost("add-message")]
        public async Task<IActionResult> AddMessage(MessageAddDto model)
        {
            var visitorIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            await UnitOfWork.HokmShelemRepository.AddMessageAsync(visitorIp, model);
            await UnitOfWork.CompleteAsync();

            var body = $"<p>Name: {model.Name}<br>Email: {model.Email}</p>" +
              $"<p>Message:<br>" +
              $"{model.Message}</p>";

            var emailSend = new EmailSendDto("hokmshelem@gmail.com", "Client Message", body);
            await EmailService.SendEmailAsync(emailSend);

            return Ok(new ApiResponse(200, title: "Message Received", message: "Thank you for contacting us, your message has been received."));
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            return Ok(Mapper.Map<IEnumerable<CountryDto>>(await UnitOfWork.CountryRepository.GetAllAsync()));
        }
    }
}
