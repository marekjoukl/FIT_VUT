using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Facades.Interfaces;

public interface IFacade<TEntity, TListModel, TDetailModel>
    where TEntity : class, IEntity
    where TListModel : IModel
    where TDetailModel : class, IModel
{
    Task DeleteAsync(Guid id);
    Task<TDetailModel?> GetAsync(Guid id);
    Task<IEnumerable<TListModel>> GetAsync();
    Task<IEnumerable<TListModel>> GetAsync(string orderBy, bool desc);
    Task<TDetailModel> SaveAsync(TDetailModel model);
}
