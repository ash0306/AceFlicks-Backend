using System.Net.Mail;
using System.Net;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Exceptions.Email;

namespace MovieBookingBackend.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The content of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("Email");
            var useremail = emailSettings["mail"];
            var password = emailSettings["password"];

            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(useremail, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage(useremail, email, subject, message)
            {
                IsBodyHtml = true
            };

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new UnableToSendMailException(ex.Message);
            }
        }
    }
}
