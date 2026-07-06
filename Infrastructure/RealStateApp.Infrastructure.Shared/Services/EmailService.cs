using RealStateApp.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace RealStateApp.Infrastructure.Shared.Services;

public class EmailService : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlMessage)
    {
        // Dummy implementation. Will be configured with SMTP later.
        await Task.CompletedTask;
    }
}
