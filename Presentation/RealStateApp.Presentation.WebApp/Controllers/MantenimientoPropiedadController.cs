using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedades;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Agente")]
public class MantenimientoPropiedadController : Controller
{
    private readonly IMantenimientoPropiedadService _mantenimientoPropiedadService;

    public MantenimientoPropiedadController(IMantenimientoPropiedadService mantenimientoPropiedadService)
    {
        _mantenimientoPropiedadService = mantenimientoPropiedadService;
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
        if (idClaim != null && int.TryParse(idClaim.Value, out int id)) return id;
        return 0; // Replace with proper extraction
    }

    public async Task<IActionResult> Index()
    {
        var propiedades = await _mantenimientoPropiedadService.GetAllViewModelWithFilters(GetCurrentUserId());
        return View(propiedades);
    }

    public async Task<IActionResult> Create()
    {
        var tiposPropiedad = await _mantenimientoPropiedadService.GetTiposPropiedad();
        var tiposVenta = await _mantenimientoPropiedadService.GetTiposVenta();
        var mejoras = await _mantenimientoPropiedadService.GetMejoras();

        if (!tiposPropiedad.Any())
        {
            TempData["Error"] = "No existen tipos de propiedades registrados. Debe crear al menos un tipo de propiedad antes de registrar una propiedad.";
            return RedirectToAction(nameof(Index));
        }
        if (!tiposVenta.Any())
        {
            TempData["Error"] = "No existen tipos de ventas registrados. Debe crear al menos un tipo de venta antes de registrar una propiedad.";
            return RedirectToAction(nameof(Index));
        }
        if (!mejoras.Any())
        {
            TempData["Error"] = "No existen mejoras registradas. Debe crear al menos una mejora antes de registrar una propiedad.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.TiposPropiedad = new SelectList(tiposPropiedad, "Id", "Nombre");
        ViewBag.TiposVenta = new SelectList(tiposVenta, "Id", "Nombre");
        ViewBag.Mejoras = mejoras;

        return View(new SavePropiedadViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(SavePropiedadViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewBags(vm);
            return View(vm);
        }

        if (vm.ImagenesFiles == null || !vm.ImagenesFiles.Any())
        {
            ModelState.AddModelError("", "Debe cargar al menos una imagen de la propiedad.");
            await LoadViewBags(vm);
            return View(vm);
        }

        if (vm.ImagenesFiles.Count > 4)
        {
            ModelState.AddModelError("", "Solo se permite registrar hasta 4 imágenes por propiedad.");
            await LoadViewBags(vm);
            return View(vm);
        }

        foreach(var file in vm.ImagenesFiles)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
            {
                ModelState.AddModelError("", "Formato de imagen no permitido. (Solo .jpg, .jpeg, .png)");
                await LoadViewBags(vm);
                return View(vm);
            }
        }

        await _mantenimientoPropiedadService.Add(vm, GetCurrentUserId());
        TempData["Success"] = "La propiedad fue creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _mantenimientoPropiedadService.GetByIdSaveViewModel(id, GetCurrentUserId());
        if (vm == null)
        {
            TempData["Error"] = "No tiene permisos para modificar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (vm.Estado == "Vendida")
        {
            TempData["Error"] = "No se puede modificar una propiedad que ya fue vendida.";
            return RedirectToAction(nameof(Index));
        }

        await LoadViewBags(vm);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SavePropiedadViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewBags(vm);
            return View(vm);
        }

        var propiedadExistente = await _mantenimientoPropiedadService.GetByIdSaveViewModel(vm.Id, GetCurrentUserId());
        if (propiedadExistente == null)
        {
            TempData["Error"] = "No tiene permisos para modificar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (propiedadExistente.Estado == "Vendida")
        {
            TempData["Error"] = "No se puede modificar una propiedad que ya fue vendida.";
            return RedirectToAction(nameof(Index));
        }

        var totalImages = propiedadExistente.ImagenesActuales.Count;
        if (vm.ImagenesFiles != null)
        {
            totalImages += vm.ImagenesFiles.Count;
            foreach(var file in vm.ImagenesFiles)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    ModelState.AddModelError("", "Formato de imagen no permitido. (Solo .jpg, .jpeg, .png)");
                    await LoadViewBags(vm);
                    return View(vm);
                }
            }
        }

        if (totalImages == 0)
        {
            ModelState.AddModelError("", "Debe cargar al menos una imagen de la propiedad.");
            await LoadViewBags(vm);
            return View(vm);
        }

        if (totalImages > 4)
        {
            ModelState.AddModelError("", "Solo se permite registrar hasta 4 imágenes por propiedad.");
            await LoadViewBags(vm);
            return View(vm);
        }

        await _mantenimientoPropiedadService.Update(vm, GetCurrentUserId());
        TempData["Success"] = "La propiedad fue actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vm = await _mantenimientoPropiedadService.GetByIdSaveViewModel(id, GetCurrentUserId());
        if (vm == null)
        {
            TempData["Error"] = "No tiene permisos para eliminar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (vm.Estado == "Vendida")
        {
            TempData["Error"] = "No se puede eliminar una propiedad que ya fue vendida.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vm = await _mantenimientoPropiedadService.GetByIdSaveViewModel(id, GetCurrentUserId());
        if (vm == null)
        {
            TempData["Error"] = "No tiene permisos para eliminar esta propiedad.";
            return RedirectToAction(nameof(Index));
        }

        if (vm.Estado == "Vendida")
        {
            TempData["Error"] = "No se puede eliminar una propiedad que ya fue vendida.";
            return RedirectToAction(nameof(Index));
        }

        await _mantenimientoPropiedadService.Delete(id, GetCurrentUserId());
        TempData["Success"] = "La propiedad fue eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadViewBags(SavePropiedadViewModel vm)
    {
        var tiposPropiedad = await _mantenimientoPropiedadService.GetTiposPropiedad();
        var tiposVenta = await _mantenimientoPropiedadService.GetTiposVenta();
        var mejoras = await _mantenimientoPropiedadService.GetMejoras();

        ViewBag.TiposPropiedad = new SelectList(tiposPropiedad, "Id", "Nombre", vm.TipoPropiedadId);
        ViewBag.TiposVenta = new SelectList(tiposVenta, "Id", "Nombre", vm.TipoVentaId);
        ViewBag.Mejoras = mejoras;
    }
}
