using EntityLayer.DTOs;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ServiceLayer.Helpers
{
	public interface IEmailSendMethod
	{
		Task SendPasswordResetToken(string token, string toEmail);
	}


	public class EmailSendMethod : IEmailSendMethod
	{
		private readonly EmailInformationDTO _emailInfo;

        public EmailSendMethod(IOptions<EmailInformationDTO> emailInfo)
        {
			_emailInfo = emailInfo.Value;
        }

        public async Task SendPasswordResetToken(string token, string toEmail)
		{
			// SmtpClient properties
			var smtpClient = new SmtpClient();

			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			smtpClient.Host = _emailInfo.Host;
			smtpClient.Port = 587;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential(_emailInfo.Email, _emailInfo.Password);
			smtpClient.EnableSsl = true;

			// mailMessage properties
			var mailMessage = new MailMessage();

			mailMessage.From = new MailAddress(_emailInfo.Email);
			mailMessage.To.Add(toEmail);
			mailMessage.Subject = "Password Reset Link";
			mailMessage.Body = $@"<h4>{token} - Use this token to change your Password </h4>";
			mailMessage.IsBodyHtml = true;

			await smtpClient.SendMailAsync(mailMessage);
		}
	}
}
