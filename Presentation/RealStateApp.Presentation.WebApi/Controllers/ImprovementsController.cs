using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Mejoras;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Presentation.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Administrador, Desarrollador")]
public class ImprovementsController : ControllerBase
{
    private readonly IMejoraService _mejoraService;

    public ImprovementsController(IMejoraService mejoraService)
    {
        _mejoraService = mejoraService;
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
            var mejoras = await _mejoraService.GetAllViewModelWithIncludeAsync();
            if (mejoras == null || !mejoras.Any())
            {
                return NoContent();
            }
            return Ok(mejoras);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Ocurrió un error interno en el servidor." });
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
            if (id <= 0) return BadRequest(new { Message = "El Id enviado no tiene un formato válido." });
            var mejoraSave = await _mejoraService.GetSaveViewModelByIdAsync(id);
            if (mejoraSave == null)
            {
                return NotFound(new { Message = "La mejora solicitada no existe." });
            }
            
            var mejora = await _mejoraService.GetViewModelByIdWithIncludeAsync(id);
            return Ok(mejora);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Ocurrió un error interno en el servidor." });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] SaveMejoraViewModel vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Los datos enviados no son válidos.", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            if (string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
            {
                return BadRequest(new { Message = "Los datos enviados no son válidos." });
            }

            var mejoras = await _mejoraService.GetAllViewModelWithIncludeAsync();
            if (mejoras.Any(m => m.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                return BadRequest(new { Message = "Ya existe una mejora registrada con este nombre." });
            }

            var result = await _mejoraService.AddAsync(vm);
            var allVm = await _mejoraService.GetAllViewModelWithIncludeAsync();
            var createdVm = allVm.FirstOrDefault(m => m.Id == result.Id);
            return StatusCode(StatusCodes.Status201Created, createdVm);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Ocurrió un error interno en el servidor." });
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
    public async Task<IActionResult> Update(int id, [FromBody] SaveMejoraViewModel vm)
    {
        try
        {
            if (id <= 0) return BadRequest(new { Message = "El Id enviado no tiene un formato válido." });
            
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Los datos enviados no son válidos.", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            if (id != vm.Id)
            {
                return BadRequest(new { Message = "Los datos enviados no son válidos." });
            }

            if (string.IsNullOrWhiteSpace(vm.Nombre) || string.IsNullOrWhiteSpace(vm.Descripcion))
            {
                return BadRequest(new { Message = "Los datos enviados no son válidos." });
            }

            var existingMejora = await _mejoraService.GetSaveViewModelByIdAsync(id);
            if (existingMejora == null)
            {
                return NotFound(new { Message = "La mejora solicitada no existe." });
            }

            var mejoras = await _mejoraService.GetAllViewModelWithIncludeAsync();
            if (mejoras.Any(m => m.Id != id && m.Nombre.Trim().ToLower() == vm.Nombre.Trim().ToLower()))
            {
                return BadRequest(new { Message = "Ya existe otra mejora registrada con este nombre." });
            }

            await _mejoraService.UpdateAsync(vm, id);
            
            var allVm = await _mejoraService.GetAllViewModelWithIncludeAsync();
            var updatedVm = allVm.FirstOrDefault(m => m.Id == id);
            return Ok(updatedVm);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Ocurrió un error interno en el servidor." });
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
            if (id <= 0) return BadRequest(new { Message = "El Id enviado no tiene un formato válido." });
            var mejora = await _mejoraService.GetSaveViewModelByIdAsync(id);
            if (mejora == null)
            {
                return NotFound(new { Message = "La mejora solicitada no existe." });
            }

            await _mejoraService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Ocurrió un error interno en el servidor." });
        }
    }
}
