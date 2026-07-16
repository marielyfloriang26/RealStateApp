using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;
using RealStateApp.Application.ViewModels.Propiedad;
using RealStateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateApp.Application.Services;

public class OfertaService : IOfertaService
{
    private readonly IOfertaRepository _ofertaRepository;
    private readonly IMapper _mapper;

    public OfertaService(IOfertaRepository ofertaRepository, IMapper mapper)
    {
        _ofertaRepository = ofertaRepository;
        _mapper = mapper;
    }

    public async Task<List<OfertaViewModel>> GetOffersByClientAndPropertyAsync(int clientId, int propiedadId)
    {
        var ofertas = await _ofertaRepository.GetAllAsync();
        var filtradas = ofertas.Where(o => o.ClienteId == clientId && o.PropiedadId == propiedadId)
        .OrderByDescending(o => o.FechaOferta).ToList();
        return _mapper.Map<List<OfertaViewModel>>(filtradas);
    }

    public async Task<bool> HasPendingOfferAsync(int clientId, int propiedadId)
    {
        var ofertas = await _ofertaRepository.GetAllAsync();
        return ofertas.Any(o => o.ClienteId == clientId && o.PropiedadId == propiedadId && o.Estado == "Pendiente");
    }

    public async Task<bool> HasAcceptedOfferAsync(int propiedadId)
    {
        var ofertas = await _ofertaRepository.GetAllAsync();
        return ofertas.Any(o => o.PropiedadId == propiedadId && o.Estado == "Aceptada");
    }

    public async Task MakeOfferAsync(int clientId, int propiedadId, decimal amount)
    {
        var oferta = new Oferta
        {
            ClienteId = clientId,
            PropiedadId = propiedadId,
            Monto = amount,
            Estado = "Pendiente",
            FechaOferta = DateTime.UtcNow
        };
        await _ofertaRepository.AddAsync(oferta);
    }
}