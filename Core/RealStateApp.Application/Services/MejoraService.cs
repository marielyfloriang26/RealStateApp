using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Mejoras;
using RealStateApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Application.Services;

public class MejoraService : IMejoraService
{
    private readonly IMejoraRepository _mejoraRepository;
    private readonly IMapper _mapper;

    public MejoraService(IMejoraRepository mejoraRepository, IMapper mapper)
    {
        _mejoraRepository = mejoraRepository;
        _mapper = mapper;
    }

    public async Task<List<MejoraViewModel>> GetAllViewModelWithIncludeAsync()
    {
        var mejoras = await _mejoraRepository.GetAllWithIncludeAsync(new List<string> { "PropiedadMejoras" });
        var viewModels = _mapper.Map<List<MejoraViewModel>>(mejoras);

        foreach (var vm in viewModels)
        {
            var mejora = mejoras.FirstOrDefault(m => m.Id == vm.Id);
            if (mejora != null)
            {
                vm.CantidadPropiedades = mejora.PropiedadMejoras?.Count ?? 0;
            }
        }
        
        return viewModels;
    }

    public async Task<MejoraViewModel?> GetViewModelByIdWithIncludeAsync(int id)
    {
        var mejoras = await _mejoraRepository.GetAllWithIncludeAsync(new List<string> { "PropiedadMejoras" });
        var mejora = mejoras.FirstOrDefault(m => m.Id == id);
        if (mejora == null) return null;

        var viewModel = _mapper.Map<MejoraViewModel>(mejora);
        viewModel.CantidadPropiedades = mejora.PropiedadMejoras?.Count ?? 0;
        return viewModel;
    }

    public async Task<SaveMejoraViewModel> AddAsync(SaveMejoraViewModel vm)
    {
        var entity = _mapper.Map<Mejora>(vm);
        await _mejoraRepository.AddAsync(entity);
        return _mapper.Map<SaveMejoraViewModel>(entity);
    }

    public async Task<SaveMejoraViewModel?> GetSaveViewModelByIdAsync(int id)
    {
        var entity = await _mejoraRepository.GetByIdAsync(id);
        if (entity == null) return null;
        return _mapper.Map<SaveMejoraViewModel>(entity);
    }

    public async Task<SaveMejoraViewModel> UpdateAsync(SaveMejoraViewModel vm, int id)
    {
        var entity = _mapper.Map<Mejora>(vm);
        entity.Id = id;
        await _mejoraRepository.UpdateAsync(entity);
        return _mapper.Map<SaveMejoraViewModel>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _mejoraRepository.GetByIdAsync(id);
        if (entity != null)
        {
            await _mejoraRepository.DeleteAsync(entity);
        }
    }
}
