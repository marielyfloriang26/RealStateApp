using System;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string body);
}
