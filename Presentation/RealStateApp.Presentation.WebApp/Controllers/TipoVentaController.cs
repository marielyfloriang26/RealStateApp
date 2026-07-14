using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.TipoVentas;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class TipoVentaController : Controller
{
    private readonly ITipoVentaService _tipoVentaService;

    public TipoVentaController(ITipoVentaService tipoVentaService)
    {
        _tipoVentaService = tipoVentaService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = await _tipoVentaService.GetAllViewModel();
        return View(vm);
    }

    public IActionResult Create()
    {
        return View(new SaveTipoVentaViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveTipoVentaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var tipos = await _tipoVentaService.GetAllViewModel();
        if (tipos.Any(t => t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
        {
            ModelState.AddModelError("Nombre", "Ya existe un tipo de venta registrado con este nombre.");
            return View(vm);
        }

        await _tipoVentaService.Add(vm);
        TempData["SuccessMessage"] = "El tipo de venta fue creado correctamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _tipoVentaService.GetByIdSaveViewModel(id);
        if (vm == null)
        {
            return RedirectToAction("Index");
        }
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SaveTipoVentaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var tipos = await _tipoVentaService.GetAllViewModel();
        if (tipos.Any(t => t.Id != vm.Id && t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
        {
            ModelState.AddModelError("Nombre", "Ya existe otro tipo de venta registrado con este nombre.");
            return View(vm);
        }

        await _tipoVentaService.Update(vm, vm.Id);
        TempData["SuccessMessage"] = "El tipo de venta fue actualizado correctamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vm = await _tipoVentaService.GetByIdSaveViewModel(id);
        if (vm == null)
        {
            TempData["ErrorMessage"] = "El tipo de venta seleccionado no existe.";
            return RedirectToAction("Index");
        }
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var vm = await _tipoVentaService.GetByIdSaveViewModel(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "El tipo de venta seleccionado no existe.";
                return RedirectToAction("Index");
            }

            await _tipoVentaService.Delete(id);
            TempData["SuccessMessage"] = "El tipo de venta fue eliminado correctamente.";
        }
        catch (System.Exception)
        {
            TempData["ErrorMessage"] = "No fue posible eliminar el tipo de venta. Intente nuevamente más tarde.";
        }
        
        return RedirectToAction("Index");
    }
}
