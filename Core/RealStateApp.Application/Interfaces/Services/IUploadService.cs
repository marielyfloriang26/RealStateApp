using Microsoft.AspNetCore.Http;

namespace RealStateApp.Application.Interfaces.Services;

public interface IUploadService
{
    string UploadFile(IFormFile file, int id);
}