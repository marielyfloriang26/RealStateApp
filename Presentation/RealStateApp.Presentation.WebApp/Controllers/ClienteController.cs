using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Application.ViewModels.TipoPropiedades;
using RealStateApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Cliente")] // para que solo entren Clientes
public class ClienteController : Controller
{
    private readonly IPropiedadService _propiedadService;
    private readonly ITipoPropiedadService _tipoPropiedadService;
    private readonly IPropiedadFavoritaService _favoritoService;
    private readonly IMensajeService _mensajeService;
    private readonly IOfertaService _ofertaService;

    public ClienteController(
        IPropiedadService propiedadService, 
        ITipoPropiedadService tipoPropiedadService, 
        IPropiedadFavoritaService favoritoService, IMensajeService mensajeService,
        IOfertaService ofertaService)
    {
        _propiedadService = propiedadService;
        _tipoPropiedadService = tipoPropiedadService;
        _favoritoService = favoritoService;
        _mensajeService = mensajeService; 
        _ofertaService = ofertaService;
    }

    public async Task<IActionResult> Index(string? searchCode, FiltroPropiedadViewModel filter)
    {
        ViewBag.TiposPropiedad = await _tipoPropiedadService.GetAllAsync();
        
        List<PropiedadViewModel> propiedades;
        var clientId = GetLoggedInClientId();

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
            if (filter.TipoPropiedadId.HasValue || 
                filter.PrecioMinimo.HasValue || 
                filter.PrecioMaximo.HasValue || 
                filter.CantidadHabitaciones.HasValue || 
                filter.CantidadBanos.HasValue)
            {
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

        // Carga la lista de IDs de propiedades favoritas del cliente logueado
        ViewBag.Favoritos = await _favoritoService.GetFavoritePropertyIdsByClientIdAsync(clientId);
        ViewBag.Filter = filter;
        ViewBag.SearchCode = searchCode;

        return View(propiedades);
    }

    public async Task<IActionResult> MisPropiedades()
    {
        var clientId = GetLoggedInClientId();
        var favoritas = await _favoritoService.GetFavoritesByClientIdAsync(clientId);
        return View(favoritas);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarFavorito(int id)
    {
        var clientId = GetLoggedInClientId();
        await _favoritoService.AddFavoriteAsync(clientId, id);
        TempData["SuccessMessage"] = "La propiedad fue agregada a sus favoritas correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EliminarFavorito(int id, string? redirectToAction)
    {
        var clientId = GetLoggedInClientId();
        await _favoritoService.RemoveFavoriteAsync(clientId, id);
        TempData["SuccessMessage"] = "La propiedad fue eliminada de sus favoritas correctamente.";

        if (redirectToAction == "MisPropiedades")
        {
            return RedirectToAction("MisPropiedades");
        }
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Details(int id)
{
    var propiedad = await _propiedadService.GetByIdWithIncludeAsync(id);
    if (propiedad == null)
    {
        ViewBag.Message = "La propiedad solicitada no existe o no se encuentra disponible.";
        return View("PropertyNotFound");
    }

    var clientId = GetLoggedInClientId();

    // Obtener historial de chat
    ViewBag.ChatHistory = await _mensajeService.GetChatHistoryAsync(clientId, id);

    // Obtener ofertas realizadas por este cliente
    ViewBag.Ofertas = await _ofertaService.GetOffersByClientAndPropertyAsync(clientId, id);

    // Validaciones de ofertas para la vista
    ViewBag.HasPendingOffer = await _ofertaService.HasPendingOfferAsync(clientId, id);
    ViewBag.HasAcceptedOffer = await _ofertaService.HasAcceptedOfferAsync(id);

    return View(propiedad);
}

[HttpPost]
public async Task<IActionResult> EnviarMensaje(int propiedadId, int agenteId, string contenido)
{
    if (string.IsNullOrWhiteSpace(contenido))
    {
        TempData["ErrorMessage"] = "Debe escribir un mensaje antes de enviarlo.";
        return RedirectToAction("Details", new { id = propiedadId });
    }

    var clientId = GetLoggedInClientId();
    await _mensajeService.SendMessageAsync(clientId, agenteId, propiedadId, contenido, "Cliente");
    return RedirectToAction("Details", new { id = propiedadId });
}

[HttpPost]
public async Task<IActionResult> EnviarOferta(int propiedadId, decimal monto)
{
    var clientId = GetLoggedInClientId();

    if (monto <= 0)
    {
        TempData["ErrorMessage"] = "El monto de la oferta debe ser un valor numérico mayor que cero.";
        return RedirectToAction("Details", new { id = propiedadId });
    }

    if (await _ofertaService.HasPendingOfferAsync(clientId, propiedadId))
    {
        TempData["ErrorMessage"] = "Ya tiene una oferta pendiente para esta propiedad.";
        return RedirectToAction("Details", new { id = propiedadId });
    }

    if (await _ofertaService.HasAcceptedOfferAsync(propiedadId))
    {
        TempData["ErrorMessage"] = "Esta propiedad ya tiene una oferta aceptada y no permite nuevas ofertas.";
        return RedirectToAction("Details", new { id = propiedadId });
    }

    await _ofertaService.MakeOfferAsync(clientId, propiedadId, monto);
    TempData["SuccessMessage"] = "Su oferta ha sido enviada correctamente.";
    return RedirectToAction("Details", new { id = propiedadId });
}

    private int GetLoggedInClientId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(claim!);
    }
}