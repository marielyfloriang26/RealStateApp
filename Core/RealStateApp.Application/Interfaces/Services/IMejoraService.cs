using RealStateApp.Application.ViewModels.Mejoras;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApp.Application.Interfaces.Services;

public interface IMejoraService
{
    Task<List<MejoraViewModel>> GetAllViewModelWithIncludeAsync();
    Task<MejoraViewModel?> GetViewModelByIdWithIncludeAsync(int id);
    Task<SaveMejoraViewModel> AddAsync(SaveMejoraViewModel vm);
    Task<SaveMejoraViewModel?> GetSaveViewModelByIdAsync(int id);
    Task<SaveMejoraViewModel> UpdateAsync(SaveMejoraViewModel vm, int id);
    Task DeleteAsync(int id);
}
