using System;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlMessage);
}
