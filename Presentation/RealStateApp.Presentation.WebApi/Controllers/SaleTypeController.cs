using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.TipoVentas;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrador, Desarrollador")]
public class SaleTypeController : ControllerBase
{
    private readonly ITipoVentaService _tipoVentaService;

    public SaleTypeController(ITipoVentaService tipoVentaService)
    {
        _tipoVentaService = tipoVentaService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> List()
    {
        try
        {
            var tipos = await _tipoVentaService.GetAllViewModel();
            if (tipos == null || !tipos.Any())
            {
                return NoContent();
            }
            return Ok(tipos);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0) return BadRequest("El Id enviado no tiene un formato válido.");
            var tipo = await _tipoVentaService.GetByIdSaveViewModel(id);
            if (tipo == null)
            {
                return NotFound("El tipo de venta solicitado no existe.");
            }
            // Return TipoVentaViewModel instead of SaveTipoVentaViewModel
            var vm = await _tipoVentaService.GetAllViewModel();
            var tipoVm = vm.FirstOrDefault(t => t.Id == id);
            return Ok(tipoVm);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] SaveTipoVentaViewModel vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Los datos enviados no son válidos.");
            }

            if (string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
            {
                return BadRequest("Los datos enviados no son válidos.");
            }

            var tipos = await _tipoVentaService.GetAllViewModel();
            if (tipos.Any(t => t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                return BadRequest("Ya existe un tipo de venta registrado con este nombre.");
            }

            var result = await _tipoVentaService.Add(vm);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] SaveTipoVentaViewModel vm)
    {
        try
        {
            if (id <= 0) return BadRequest("El Id enviado no tiene un formato válido.");
            
            if (!ModelState.IsValid)
            {
                return BadRequest("Los datos enviados no son válidos.");
            }

            if (id != vm.Id)
            {
                return BadRequest("Los datos enviados no son válidos.");
            }

            if (string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
            {
                return BadRequest("Los datos enviados no son válidos.");
            }

            var existingTipo = await _tipoVentaService.GetByIdSaveViewModel(id);
            if (existingTipo == null)
            {
                return NotFound("El tipo de venta solicitado no existe.");
            }

            var tipos = await _tipoVentaService.GetAllViewModel();
            if (tipos.Any(t => t.Id != id && t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                return BadRequest("Ya existe otro tipo de venta registrado con este nombre.");
            }

            await _tipoVentaService.Update(vm, id);
            
            var allVm = await _tipoVentaService.GetAllViewModel();
            var updatedVm = allVm.FirstOrDefault(t => t.Id == id);
            return Ok(updatedVm);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0) return BadRequest("El Id enviado no tiene un formato válido.");
            var tipo = await _tipoVentaService.GetByIdSaveViewModel(id);
            if (tipo == null)
            {
                return NotFound("El tipo de venta solicitado no existe.");
            }

            await _tipoVentaService.Delete(id);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }
}
