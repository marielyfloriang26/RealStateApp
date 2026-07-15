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
        var port = int.Parse(smtpSettings["Port"] ?? throw new InvalidOperationException("SmtpSettings:Port no está configurado."));
        var client = new SmtpClient(smtpSettings["Host"], port)
        {
            Credentials = new NetworkCredential(smtpSettings["User"], smtpSettings["Pass"]),
            EnableSsl = true
        };

        var from = smtpSettings["From"] ?? throw new InvalidOperationException("SmtpSettings:From no está configurado.");
        var mailMessage = new MailMessage(from, email, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}
