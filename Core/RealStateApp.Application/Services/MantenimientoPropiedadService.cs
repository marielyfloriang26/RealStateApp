using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedades;
using RealStateApp.Domain.Entities;

namespace RealStateApp.Application.Services;

public class MantenimientoPropiedadService : IMantenimientoPropiedadService
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly ITipoPropiedadRepository _tipoPropiedadRepository;
    private readonly ITipoVentaRepository _tipoVentaRepository;
    private readonly IMejoraRepository _mejoraRepository;
    private readonly IImagenPropiedadRepository _imagenPropiedadRepository;
    private readonly IPropiedadMejoraRepository _propiedadMejoraRepository;

    public MantenimientoPropiedadService(
        IPropiedadRepository propiedadRepository,
        ITipoPropiedadRepository tipoPropiedadRepository,
        ITipoVentaRepository tipoVentaRepository,
        IMejoraRepository mejoraRepository,
        IImagenPropiedadRepository imagenPropiedadRepository,
        IPropiedadMejoraRepository propiedadMejoraRepository)
    {
        _propiedadRepository = propiedadRepository;
        _tipoPropiedadRepository = tipoPropiedadRepository;
        _tipoVentaRepository = tipoVentaRepository;
        _mejoraRepository = mejoraRepository;
        _imagenPropiedadRepository = imagenPropiedadRepository;
        _propiedadMejoraRepository = propiedadMejoraRepository;
    }

    public async Task<List<PropiedadViewModel>> GetAllViewModelWithFilters(int agenteId)
    {
        var propiedades = await _propiedadRepository.GetAllWithIncludeAsync(new List<string> { "TipoPropiedad", "TipoVenta", "Imagenes" });
        
        return propiedades
            .Where(p => p.AgenteId == agenteId && p.Estado == "Disponible")
            .Select(p => new PropiedadViewModel
            {
                Id = p.Id,
                Codigo = p.Codigo,
                TipoPropiedadNombre = p.TipoPropiedad?.Nombre ?? "",
                TipoVentaNombre = p.TipoVenta?.Nombre ?? "",
                Precio = p.Precio,
                CantidadHabitaciones = p.CantidadHabitaciones,
                CantidadBanos = p.CantidadBanos,
                TamanoMetros = p.TamanoMetros,
                ImagenPrincipalUrl = p.Imagenes?.FirstOrDefault()?.ImagenUrl ?? "",
                Estado = p.Estado
            })
            .ToList();
    }

    public async Task<SavePropiedadViewModel> Add(SavePropiedadViewModel vm, int agenteId)
    {
        string codigo = GenerarCodigoUnico();
        while (await CodigoExiste(codigo))
        {
            codigo = GenerarCodigoUnico();
        }

        var propiedad = new Propiedad
        {
            Codigo = codigo,
            TipoPropiedadId = vm.TipoPropiedadId,
            TipoVentaId = vm.TipoVentaId,
            AgenteId = agenteId,
            Precio = vm.Precio,
            Descripcion = vm.Descripcion,
            TamanoMetros = vm.TamanoMetros,
            CantidadHabitaciones = vm.CantidadHabitaciones,
            CantidadBanos = vm.CantidadBanos,
            Estado = "Disponible",
            FechaCreacion = DateTime.UtcNow
        };

        var savedPropiedad = await _propiedadRepository.AddAsync(propiedad);

        if (vm.MejorasIds != null && vm.MejorasIds.Any())
        {
            foreach (var mejoraId in vm.MejorasIds)
            {
                await _propiedadMejoraRepository.AddAsync(new PropiedadMejora
                {
                    PropiedadId = savedPropiedad.Id,
                    MejoraId = mejoraId
                });
            }
        }

        if (vm.ImagenesFiles != null && vm.ImagenesFiles.Any())
        {
            foreach (var file in vm.ImagenesFiles)
            {
                string imageUrl = await UploadImage(file, savedPropiedad.Id);
                await _imagenPropiedadRepository.AddAsync(new ImagenPropiedad
                {
                    PropiedadId = savedPropiedad.Id,
                    ImagenUrl = imageUrl
                });
            }
        }

        vm.Id = savedPropiedad.Id;
        return vm;
    }

    public async Task Update(SavePropiedadViewModel vm, int agenteId)
    {
        var propiedad = await _propiedadRepository.GetByIdAsync(vm.Id);
        if (propiedad == null || propiedad.AgenteId != agenteId || propiedad.Estado == "Vendida")
            return;

        propiedad.TipoPropiedadId = vm.TipoPropiedadId;
        propiedad.TipoVentaId = vm.TipoVentaId;
        propiedad.Precio = vm.Precio;
        propiedad.Descripcion = vm.Descripcion;
        propiedad.TamanoMetros = vm.TamanoMetros;
        propiedad.CantidadHabitaciones = vm.CantidadHabitaciones;
        propiedad.CantidadBanos = vm.CantidadBanos;

        await _propiedadRepository.UpdateAsync(propiedad);

        // Update Mejoras
        var existingMejoras = await _propiedadMejoraRepository.GetAllAsync();
        var propertyMejoras = existingMejoras.Where(m => m.PropiedadId == vm.Id).ToList();
        
        foreach (var m in propertyMejoras)
        {
            await _propiedadMejoraRepository.DeleteAsync(m);
        }

        if (vm.MejorasIds != null)
        {
            foreach (var mejoraId in vm.MejorasIds)
            {
                await _propiedadMejoraRepository.AddAsync(new PropiedadMejora
                {
                    PropiedadId = vm.Id,
                    MejoraId = mejoraId
                });
            }
        }

        // Add new images
        if (vm.ImagenesFiles != null && vm.ImagenesFiles.Any())
        {
            foreach (var file in vm.ImagenesFiles)
            {
                string imageUrl = await UploadImage(file, vm.Id);
                await _imagenPropiedadRepository.AddAsync(new ImagenPropiedad
                {
                    PropiedadId = vm.Id,
                    ImagenUrl = imageUrl
                });
            }
        }
    }

    public async Task Delete(int id, int agenteId)
    {
        var propiedad = await _propiedadRepository.GetByIdAsync(id);
        if (propiedad == null || propiedad.AgenteId != agenteId || propiedad.Estado == "Vendida")
            return;

        // Optionally delete physical image files here...

        await _propiedadRepository.DeleteAsync(propiedad);
    }

    public async Task<SavePropiedadViewModel?> GetByIdSaveViewModel(int id, int agenteId)
    {
        var propiedades = await _propiedadRepository.GetAllWithIncludeAsync(new List<string> { "PropiedadMejoras", "Imagenes" });
        var propiedad = propiedades.FirstOrDefault(p => p.Id == id && p.AgenteId == agenteId);

        if (propiedad == null) return null;

        return new SavePropiedadViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            Estado = propiedad.Estado,
            TipoPropiedadId = propiedad.TipoPropiedadId,
            TipoVentaId = propiedad.TipoVentaId,
            Precio = propiedad.Precio,
            Descripcion = propiedad.Descripcion,
            TamanoMetros = propiedad.TamanoMetros,
            CantidadHabitaciones = propiedad.CantidadHabitaciones,
            CantidadBanos = propiedad.CantidadBanos,
            MejorasIds = propiedad.PropiedadMejoras?.Select(pm => pm.MejoraId).ToList() ?? new List<int>(),
            ImagenesActuales = propiedad.Imagenes?.Select(i => i.ImagenUrl).ToList() ?? new List<string>()
        };
    }

    public async Task<List<TipoPropiedadViewModel>> GetTiposPropiedad()
    {
        var items = await _tipoPropiedadRepository.GetAllAsync();
        return items.Select(i => new TipoPropiedadViewModel { Id = i.Id, Nombre = i.Nombre }).ToList();
    }

    public async Task<List<TipoVentaViewModel>> GetTiposVenta()
    {
        var items = await _tipoVentaRepository.GetAllAsync();
        return items.Select(i => new TipoVentaViewModel { Id = i.Id, Nombre = i.Nombre }).ToList();
    }

    public async Task<List<MejoraViewModel>> GetMejoras()
    {
        var items = await _mejoraRepository.GetAllAsync();
        return items.Select(i => new MejoraViewModel { Id = i.Id, Nombre = i.Nombre }).ToList();
    }

    private string GenerarCodigoUnico()
    {
        Random rnd = new Random();
        return rnd.Next(100000, 999999).ToString();
    }

    private async Task<bool> CodigoExiste(string codigo)
    {
        var props = await _propiedadRepository.GetAllAsync();
        return props.Any(p => p.Codigo == codigo);
    }

    private async Task<string> UploadImage(IFormFile file, int propiedadId)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "propiedades", propiedadId.ToString());
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var path = Path.Combine(basePath, fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/images/propiedades/{propiedadId}/{fileName}";
    }
}
