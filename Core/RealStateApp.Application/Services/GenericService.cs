using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Application.Interfaces.Services;

namespace RealStateApp.Application.Services;

public class GenericService<SaveViewModel, ViewModel, Model> : IGenericService<SaveViewModel, ViewModel, Model>
    where SaveViewModel : class
    where ViewModel : class
    where Model : class
{
    private readonly IRepositoryAsync<Model> _repository;
    private readonly IMapper _mapper;

    public GenericService(IRepositoryAsync<Model> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<SaveViewModel> Add(SaveViewModel vm)
    {
        Model entity = _mapper.Map<Model>(vm);
        entity = await _repository.AddAsync(entity);
        SaveViewModel saveVm = _mapper.Map<SaveViewModel>(entity);
        return saveVm;
    }

    public virtual async Task Update(SaveViewModel vm, int id)
    {
        Model entity = _mapper.Map<Model>(vm);
        await _repository.UpdateAsync(entity);
    }

    public virtual async Task Delete(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            await _repository.DeleteAsync(entity);
        }
    }

    public virtual async Task<SaveViewModel?> GetByIdSaveViewModel(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;
        SaveViewModel vm = _mapper.Map<SaveViewModel>(entity);
        return vm;
    }

    public virtual async Task<List<ViewModel>> GetAllViewModel()
    {
        var entityList = await _repository.GetAllAsync();
        return _mapper.Map<List<ViewModel>>(entityList);
    }
}
