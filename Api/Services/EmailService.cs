namespace Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly SendGridOptions _sendGridOptions;
        private readonly MailJetOptions _mailJetOptions;

        public EmailService(IConfiguration config,
            IOptions<SendGridOptions> sendGridOptions,
            IOptions<MailJetOptions> mailJetOptions)
        {
            _config = config;
            _sendGridOptions = sendGridOptions.Value;
            _mailJetOptions = mailJetOptions.Value;
        }

        public async Task<bool> SendEmailAsync(EmailSendDto emailSend)
        {
            try
            {
                if (_config["Email:Provider"].Equals(SD.SMTP))
                {
                    return await SMTPSendEmailAsync(emailSend);
                }
                else if (_config["Email:Provider"].Equals(SD.SendGrid))
                {
                    return await SendGridSendEmailAsync(emailSend);
                }
                else
                {
                    return await MailJetSendEmailAsync(emailSend);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SMTPSendEmailAsync(EmailSendDto emailSend)
        {
            try
            {
                var client = new SmtpClient("mail.hokmshelem.com", 8889)
                {
                    EnableSsl = false,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_config["SMTP:Username"], _config["SMTP:Password"])
                };

                var message = new MailMessage(from: _config["SMTP:Username"],
                    to: emailSend.To, subject: emailSend.Subject, body: emailSend.Body);

                message.IsBodyHtml = true;
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SendGridSendEmailAsync(EmailSendDto emailSend)
        {
            var client = new SendGridClient(_sendGridOptions.ApiKey);
            var message = new SendGridMessage()
            {
                From = new EmailAddress(_config["Email:From"], _sendGridOptions.User),
                Subject = emailSend.Subject,
                PlainTextContent = emailSend.Body,
                HtmlContent = emailSend.Body
            };

            message.AddTo(new EmailAddress(emailSend.To));
            message.SetClickTracking(false, false);

            var result = await client.SendEmailAsync(message);

            if (result.IsSuccessStatusCode) return true;

            return false;
        }

        private async Task<bool> MailJetSendEmailAsync(EmailSendDto emailSend)
        {
            MailjetClient client = new MailjetClient(_mailJetOptions.ApiKey, _mailJetOptions.SecretKey);

            var email = new TransactionalEmailBuilder()
               .WithFrom(new SendContact(_config["Email:From"], _mailJetOptions.User))
               .WithSubject(emailSend.Subject)
               .WithHtmlPart(emailSend.Body)
               .WithTo(new SendContact(emailSend.To))
               .Build();

            var response = await client.SendTransactionalEmailAsync(email);
            if (response.Messages != null)
            {
                if (response.Messages[0].Status == "success")
                {
                    return true;
                }
            }

            return false;
        }
    }
}


