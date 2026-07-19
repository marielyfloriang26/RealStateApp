using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.TipoPropiedades;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrador, Desarrollador")]
public class PropertyTypesController : ControllerBase
{
    private readonly ITipoPropiedadService _tipoPropiedadService;

    public PropertyTypesController(ITipoPropiedadService tipoPropiedadService)
    {
        _tipoPropiedadService = tipoPropiedadService;
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
            var tipos = await _tipoPropiedadService.GetAllViewModel();
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
            var tipo = await _tipoPropiedadService.GetByIdSaveViewModel(id);
            if (tipo == null)
            {
                return NotFound("El tipo de propiedad solicitado no existe.");
            }
            
            var vm = await _tipoPropiedadService.GetAllViewModel();
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
    public async Task<IActionResult> Create([FromBody] SaveTipoPropiedadViewModel vm)
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

            var tipos = await _tipoPropiedadService.GetAllViewModel();
            if (tipos.Any(t => t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                return BadRequest("Ya existe un tipo de propiedad registrado con este nombre.");
            }

            vm.Nombre = vm.Nombre.Trim();
            vm.Descripcion = vm.Descripcion.Trim();

            var result = await _tipoPropiedadService.Add(vm);
            var refreshedTipos = await _tipoPropiedadService.GetAllViewModel();
            var createdVm = refreshedTipos.FirstOrDefault(t => t.Id == result.Id);
            return StatusCode(StatusCodes.Status201Created, createdVm);
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
    public async Task<IActionResult> Update(int id, [FromBody] SaveTipoPropiedadViewModel vm)
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

            var existingTipo = await _tipoPropiedadService.GetByIdSaveViewModel(id);
            if (existingTipo == null)
            {
                return NotFound("El tipo de propiedad solicitado no existe.");
            }

            var tipos = await _tipoPropiedadService.GetAllViewModel();
            if (tipos.Any(t => t.Id != id && t.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                return BadRequest("Ya existe otro tipo de propiedad registrado con este nombre.");
            }

            vm.Nombre = vm.Nombre.Trim();
            vm.Descripcion = vm.Descripcion.Trim();

            await _tipoPropiedadService.Update(vm, id);
            
            var allVm = await _tipoPropiedadService.GetAllViewModel();
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
            var tipo = await _tipoPropiedadService.GetByIdSaveViewModel(id);
            if (tipo == null)
            {
                return NotFound("El tipo de propiedad solicitado no existe.");
            }

            await _tipoPropiedadService.Delete(id);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
        }
    }
}
