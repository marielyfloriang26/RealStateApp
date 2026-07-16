using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Application.Services;

public class AgenteService : IAgenteService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IMapper _mapper;

    public AgenteService(UserManager<Usuario> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<AgenteViewModel>> GetAllActiveAsync()
    {
        var usuarios = await _userManager.Users.Include(u => u.Propiedades).Where(u => u.TipoUsuario == "Agente" && u.EsActivo).ToListAsync();

        var agentes = _mapper.Map<List<AgenteViewModel>>(usuarios);
        return agentes.OrderBy(a => a.Nombre).ToList();
    }

    public async Task<List<AgenteViewModel>> SearchByNameAsync(string name)
    {
        var agentes = await GetAllActiveAsync();

        if (string.IsNullOrWhiteSpace(name))
        {
            return agentes;
        }

        return agentes.Where(a => a.Nombre.Contains(name, StringComparison.OrdinalIgnoreCase) || a.Apellido.Contains(name, StringComparison.OrdinalIgnoreCase))
        .ToList();
    }

    public async Task<AgenteViewModel?> GetByIdAsync(int id)
    {
        var usuario = await _userManager.Users
            .Include(u => u.Propiedades)
            .FirstOrDefaultAsync(u => u.Id == id && u.TipoUsuario == "Agente" && u.EsActivo);

        if (usuario == null)
        {
            return null;
        }

        return _mapper.Map<AgenteViewModel>(usuario);
    }
        public async Task UpdateProfileAsync(int agentId, MiPerfilViewModel vm)
    {
        var usuario = await _userManager.FindByIdAsync(agentId.ToString());
        if (usuario != null)
        {
            usuario.Nombre = vm.Nombre;
            usuario.Apellido = vm.Apellido;
            usuario.PhoneNumber = vm.Teléfono;
            
            if (!string.IsNullOrEmpty(vm.FotoUrl))
            {
                usuario.FotoUrl = vm.FotoUrl;
            }

            await _userManager.UpdateAsync(usuario);
        }
    }
}