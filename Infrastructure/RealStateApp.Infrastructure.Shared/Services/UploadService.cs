using Microsoft.AspNetCore.Http;
using RealStateApp.Application.Interfaces.Services;
using SixLabors.ImageSharp; // Necesario para validar que es una imagen real

namespace RealStateApp.Infrastructure.Shared.Services;

public class UploadService : IUploadService
{
    public string UploadFile(IFormFile file, int id)
    {
        if (file == null || file.Length == 0)
            throw new Exception("Debe seleccionar un archivo de imagen válido.");

        // 1. Validar extensión
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            throw new Exception("El archivo seleccionado no tiene un formato de imagen válido.");

        // 2. NUEVA VALIDACIÓN: ¿Es realmente una imagen?
        try
        {
            using var image = Image.Load(file.OpenReadStream());
        }
        catch
        {
            throw new Exception("El archivo seleccionado está corrupto o no es una imagen válida.");
        }

        // 3. Directorio
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Users", id.ToString());
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

        // 4. Nombre único y guardado
        string fileName = Guid.NewGuid().ToString() + extension;
        string fullPath = Path.Combine(basePath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return $"/Images/Users/{id}/{fileName}";
    }
}