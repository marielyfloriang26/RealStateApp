using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Mejoras;
using System.Threading.Tasks;
using System.Linq;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class MejoraController : Controller
{
    private readonly IMejoraService _mejoraService;

    public MejoraController(IMejoraService mejoraService)
    {
        _mejoraService = mejoraService;
    }

    public async Task<IActionResult> Index()
    {
        var mejoras = await _mejoraService.GetAllViewModelWithIncludeAsync();
        return View(mejoras);
    }

    public IActionResult Create()
    {
        return View(new SaveMejoraViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveMejoraViewModel vm)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
        {
            vm.HasError = true;
            vm.Error = "Debe completar todos los campos requeridos.";
            return View(vm);
        }

        var mejoras = await _mejoraService.GetAllViewModelWithIncludeAsync();
        if (mejoras.Any(m => m.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
        {
            vm.HasError = true;
            vm.Error = "Ya existe una mejora registrada con este nombre.";
            return View(vm);
        }

        await _mejoraService.AddAsync(vm);
        TempData["SuccessMessage"] = "La mejora fue creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _mejoraService.GetSaveViewModelByIdAsync(id);
        if (vm == null)
        {
            TempData["ErrorMessage"] = "La mejora seleccionada no existe.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SaveMejoraViewModel vm)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
        {
            vm.HasError = true;
            vm.Error = "Debe completar todos los campos requeridos.";
            return View(vm);
        }

        var mejoras = await _mejoraService.GetAllViewModelWithIncludeAsync();
        if (mejoras.Any(m => m.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower() && m.Id != vm.Id))
        {
            vm.HasError = true;
            vm.Error = "Ya existe otra mejora registrada con este nombre.";
            return View(vm);
        }

        await _mejoraService.UpdateAsync(vm, vm.Id);
        TempData["SuccessMessage"] = "La mejora fue actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vm = await _mejoraService.GetSaveViewModelByIdAsync(id);
        if (vm == null)
        {
            TempData["ErrorMessage"] = "La mejora seleccionada no existe.";
            return RedirectToAction(nameof(Index));
        }

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var vm = await _mejoraService.GetSaveViewModelByIdAsync(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "La mejora seleccionada no existe.";
                return RedirectToAction(nameof(Index));
            }

            await _mejoraService.DeleteAsync(id);
            TempData["SuccessMessage"] = "La mejora fue eliminada correctamente.";
        }
        catch
        {
            TempData["ErrorMessage"] = "No fue posible eliminar la mejora. Intente nuevamente más tarde.";
        }

        return RedirectToAction(nameof(Index));
    }
}
