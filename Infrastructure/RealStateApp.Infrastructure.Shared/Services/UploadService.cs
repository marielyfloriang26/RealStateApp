using Microsoft.AspNetCore.Http;
using RealStateApp.Application.Interfaces.Services;

namespace RealStateApp.Infrastructure.Shared.Services;

public class UploadService : IUploadService
{
    public string UploadFile(IFormFile file, int id)
    {
        // 1. Validar que el archivo no esté vacío
        if (file == null || file.Length == 0)
        {
            throw new Exception("Debe seleccionar un archivo de imagen válido.");
        }

        // 2. Validar formatos permitidos
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
        {
            throw new Exception("El archivo seleccionado no tiene un formato de imagen válido.");
        }

        // 3. Crear el directorio si no existe
        // Nota: Usamos Path.Combine para ser cross-platform
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Users", id.ToString());
        
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        // 4. Generar nombre único para evitar colisiones
        string fileName = Guid.NewGuid().ToString() + extension;
        string fullPath = Path.Combine(basePath, fileName);

        // 5. Guardar el archivo
        try
        {
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }
        catch
        {
            throw new Exception("No fue posible completar el registro. Intente nuevamente más tarde.");
        }

        // 6. Retornar la ruta relativa para guardar en la base de datos
        return $"/Images/Users/{id}/{fileName}";
    }
}