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
public class AgentsController : ControllerBase
{
    private readonly IAgenteService _agenteService;
    private readonly IPropiedadService _propiedadService;

    public AgentsController(IAgenteService agenteService, IPropiedadService propiedadService)
    {
        _agenteService = agenteService;
        _propiedadService = propiedadService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AgenteDTO>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> List()
    {
        try
        {
            var agentes = await _agenteService.GetAllApiAsync();
            if (agentes == null || !agentes.Any())
            {
                return NoContent();
            }

            var dtos = agentes.Select(a => new AgenteDTO
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Apellido = a.Apellido,
                CantidadPropiedades = a.CantidadPropiedades,
                Correo = a.Correo,
                Telefono = a.Telefono ?? "",
                Estado = a.EsActivo
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgenteDTO))]
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

            var agente = await _agenteService.GetByIdApiAsync(id);
            if (agente == null)
            {
                return NotFound("El agente solicitado no existe.");
            }

            var dto = new AgenteDTO
            {
                Id = agente.Id,
                Nombre = agente.Nombre,
                Apellido = agente.Apellido,
                CantidadPropiedades = agente.CantidadPropiedades,
                Correo = agente.Correo,
                Telefono = agente.Telefono ?? "",
                Estado = agente.EsActivo
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpGet("{id}/properties")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropiedadDTO>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAgentProperty(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("El Id enviado no tiene un formato válido.");
            }

            var agente = await _agenteService.GetByIdApiAsync(id);
            if (agente == null)
            {
                return NotFound("El agente solicitado no existe.");
            }

            var propiedades = await _propiedadService.GetPropertiesByAgentIdAsync(id);
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

    [Authorize(Roles = "Administrador")] // Solo el Administrador puede cambiar el estado de un agente
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeAgentStatusRequest request)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("El Id enviado no tiene un formato válido.");
            }

            if (request == null)
            {
                return BadRequest("El estado enviado no es válido.");
            }

            var agente = await _agenteService.GetByIdApiAsync(id);
            if (agente == null)
            {
                return NotFound("El agente solicitado no existe.");
            }

            await _agenteService.ChangeStatusAsync(id, request.Estado);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }
}