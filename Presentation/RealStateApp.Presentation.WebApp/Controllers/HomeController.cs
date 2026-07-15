using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RealStateApp.Presentation.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IPropiedadService _propiedadService;
    private readonly ITipoPropiedadService _tipoPropiedadService;

    public HomeController(IPropiedadService propiedadService, ITipoPropiedadService tipoPropiedadService)
    {
        _propiedadService = propiedadService;
        _tipoPropiedadService = tipoPropiedadService;
    }

    public async Task<IActionResult> Index(string? searchCode, FiltroPropiedadViewModel filter)
    {
        ViewBag.TiposPropiedad = await _tipoPropiedadService.GetAllAsync();
        
        List<PropiedadViewModel> propiedades;

        if (!string.IsNullOrWhiteSpace(searchCode))
        {
            var propiedad = await _propiedadService.GetByCodeWithIncludeAsync(searchCode);
            propiedades = propiedad != null ? new List<PropiedadViewModel> { propiedad } : new List<PropiedadViewModel>();
            
            if (propiedad == null)
            {
                ViewBag.ErrorMessage = "No se encontró ninguna propiedad disponible con el código ingresado.";
            }
        }
        else
        {
            // Si hay filtros aplicados
            if (filter.TipoPropiedadId.HasValue || filter.PrecioMinimo.HasValue ||  filter.PrecioMaximo.HasValue ||  filter.CantidadHabitaciones.HasValue ||  filter.CantidadBanos.HasValue)
            {
                // Validaciones adicionales de filtros 
                if (filter.PrecioMinimo.HasValue && filter.PrecioMinimo < 0)
                {
                    ModelState.AddModelError("PrecioMinimo", "El precio mínimo no puede ser menor que cero.");
                }
                if (filter.PrecioMaximo.HasValue && filter.PrecioMaximo < 0)
                {
                    ModelState.AddModelError("PrecioMaximo", "El precio máximo no puede ser menor que cero.");
                }
                if (filter.PrecioMinimo.HasValue && filter.PrecioMaximo.HasValue && filter.PrecioMinimo > filter.PrecioMaximo)
                {
                    ModelState.AddModelError("PrecioMinimo", "El precio mínimo no puede ser mayor que el precio máximo.");
                }

                if (ModelState.IsValid)
                {
                    propiedades = await _propiedadService.GetAllFilteredAsync(filter);
                    if (propiedades.Count == 0)
                    {
                        ViewBag.ErrorMessage = "No se encontraron propiedades disponibles con los filtros seleccionados.";
                    }
                }
                else
                {
                    propiedades = await _propiedadService.GetAllWithIncludeAsync();
                }
            }
            else
            {
                propiedades = await _propiedadService.GetAllWithIncludeAsync();
            }
        }

        ViewBag.Filter = filter;
        ViewBag.SearchCode = searchCode;

        return View(propiedades);
    }

    public async Task<IActionResult> Details(int id)
    {
        var propiedad = await _propiedadService.GetByIdWithIncludeAsync(id);
        if (propiedad == null)
        {
            return View("PropertyNotFound");
        }
        return View(propiedad);
    }
}