using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using RealStateApp.Application.Interfaces.Services;
using System.Net;


namespace RealStateApp.Infrastructure.Shared.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string email, string subject, string body)
    {
        var smtpSettings = _config.GetSection("SmtpSettings");
        var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
        {
            Credentials = new NetworkCredential(smtpSettings["User"], smtpSettings["Pass"]),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(smtpSettings["From"], email, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}
