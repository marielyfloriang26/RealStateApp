using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Services;

public class TipoPropiedadService : ITipoPropiedadService
{
    private readonly ITipoPropiedadRepository _tipoPropiedadRepository;
    private readonly IMapper _mapper;

    public TipoPropiedadService(ITipoPropiedadRepository tipoPropiedadRepository, IMapper mapper)
    {
        _tipoPropiedadRepository = tipoPropiedadRepository;
        _mapper = mapper;
    }

    public async Task<List<TipoPropiedadViewModel>> GetAllAsync()
    {
        var list = await _tipoPropiedadRepository.GetAllAsync();
        return _mapper.Map<List<TipoPropiedadViewModel>>(list);
    }
}