using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Presentation.WebApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApi.Controllers;

[Authorize(Roles = "Administrador, Desarrollador")] // Filtro JWT para estos dos roles
[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropiedadService _propiedadService;

    public PropertiesController(IPropiedadService propiedadService)
    {
        _propiedadService = propiedadService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropiedadDTO>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> List()
    {
        try
        {
            var propiedades = await _propiedadService.GetAllApiAsync();
            if (propiedades == null || !propiedades.Any())
            {
                return NoContent();
            }

            var dtos = propiedades.Select(p => new PropiedadDTO
            {
                Id = p.Id,
                Codigo = p.Codigo,
                TipoPropiedad = p.TipoPropiedadNombre,
                TipoVenta = p.TipoVentaNombre,
                Precio = p.Precio,
                TamanoMetros = p.TamanoMetros,
                CantidadHabitaciones = p.CantidadHabitaciones,
                CantidadBanos = p.CantidadBanos,
                Descripcion = p.Descripcion,
                Mejoras = p.Mejoras ?? new List<string>(),
                Agente = $"{p.AgenteNombre} {p.AgenteApellido}".Trim(),
                AgenteId = p.AgenteId,
                Estado = p.Estado
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropiedadDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("El Id enviado no tiene un formato válido.");
            }

            var propiedad = await _propiedadService.GetByIdApiAsync(id);
            if (propiedad == null)
            {
                return NotFound("La propiedad solicitada no existe.");
            }

            var dto = new PropiedadDTO
            {
                Id = propiedad.Id,
                Codigo = propiedad.Codigo,
                TipoPropiedad = propiedad.TipoPropiedadNombre,
                TipoVenta = propiedad.TipoVentaNombre,
                Precio = propiedad.Precio,
                TamanoMetros = propiedad.TamanoMetros,
                CantidadHabitaciones = propiedad.CantidadHabitaciones,
                CantidadBanos = propiedad.CantidadBanos,
                Descripcion = propiedad.Descripcion,
                Mejoras = propiedad.Mejoras ?? new List<string>(),
                Agente = $"{propiedad.AgenteNombre} {propiedad.AgenteApellido}".Trim(),
                AgenteId = propiedad.AgenteId,
                Estado = propiedad.Estado
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropiedadDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 6 || !code.All(char.IsDigit))
            {
                return BadRequest("El código enviado no tiene un formato válido.");
            }

            var propiedad = await _propiedadService.GetByCodeApiAsync(code);
            if (propiedad == null)
            {
                return NotFound("No existe una propiedad registrada con el código enviado.");
            }

            var dto = new PropiedadDTO
            {
                Id = propiedad.Id,
                Codigo = propiedad.Codigo,
                TipoPropiedad = propiedad.TipoPropiedadNombre,
                TipoVenta = propiedad.TipoVentaNombre,
                Precio = propiedad.Precio,
                TamanoMetros = propiedad.TamanoMetros,
                CantidadHabitaciones = propiedad.CantidadHabitaciones,
                CantidadBanos = propiedad.CantidadBanos,
                Descripcion = propiedad.Descripcion,
                Mejoras = propiedad.Mejoras ?? new List<string>(),
                Agente = $"{propiedad.AgenteNombre} {propiedad.AgenteApellido}".Trim(),
                AgenteId = propiedad.AgenteId,
                Estado = propiedad.Estado
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }
}