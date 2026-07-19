using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.TipoPropiedades;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class TipoPropiedadController : Controller
{
    private readonly ITipoPropiedadService _tipoPropiedadService;

    public TipoPropiedadController(ITipoPropiedadService tipoPropiedadService)
    {
        _tipoPropiedadService = tipoPropiedadService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = await _tipoPropiedadService.GetAllViewModel();
        return View(vm);
    }

    public IActionResult Create()
    {
        return View(new SaveTipoPropiedadViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveTipoPropiedadViewModel vm)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
        {
            if (string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
            {
                if (!ModelState.ContainsKey("Nombre") && !ModelState.ContainsKey("Descripcion"))
                {
                    ModelState.AddModelError("", "Debe completar todos los campos requeridos.");
                }
            }
            return View(vm);
        }

        var tipos = await _tipoPropiedadService.GetAllViewModel();
        if (tipos.Any(t => t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
        {
            ModelState.AddModelError("Nombre", "Ya existe un tipo de propiedad registrado con este nombre.");
            return View(vm);
        }

        await _tipoPropiedadService.Add(vm);
        TempData["SuccessMessage"] = "El tipo de propiedad fue creado correctamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _tipoPropiedadService.GetByIdSaveViewModel(id);
        if (vm == null)
        {
            return RedirectToAction("Index");
        }
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SaveTipoPropiedadViewModel vm)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
        {
            if (string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
            {
                if (!ModelState.ContainsKey("Nombre") && !ModelState.ContainsKey("Descripcion"))
                {
                    ModelState.AddModelError("", "Debe completar todos los campos requeridos.");
                }
            }
            return View(vm);
        }

        var existing = await _tipoPropiedadService.GetByIdSaveViewModel(vm.Id);
        if (existing == null)
        {
            TempData["ErrorMessage"] = "El tipo de propiedad seleccionado no existe.";
            return RedirectToAction("Index");
        }

        var tipos = await _tipoPropiedadService.GetAllViewModel();
        if (tipos.Any(t => t.Id != vm.Id && t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
        {
            ModelState.AddModelError("Nombre", "Ya existe otro tipo de propiedad registrado con este nombre.");
            return View(vm);
        }

        await _tipoPropiedadService.Update(vm, vm.Id);
        TempData["SuccessMessage"] = "El tipo de propiedad fue actualizado correctamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vm = await _tipoPropiedadService.GetByIdSaveViewModel(id);
        if (vm == null)
        {
            TempData["ErrorMessage"] = "El tipo de propiedad seleccionado no existe.";
            return RedirectToAction("Index");
        }
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var vm = await _tipoPropiedadService.GetByIdSaveViewModel(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "El tipo de propiedad seleccionado no existe.";
                return RedirectToAction("Index");
            }

            await _tipoPropiedadService.Delete(id);
            TempData["SuccessMessage"] = "El tipo de propiedad fue eliminado correctamente.";
        }
        catch (System.Exception)
        {
            TempData["ErrorMessage"] = "No fue posible eliminar el tipo de propiedad. Intente nuevamente más tarde.";
        }
        
        return RedirectToAction("Index");
    }
}
