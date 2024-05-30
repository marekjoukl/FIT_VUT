using System.Collections;
using System.Reflection;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;
using InformationSystem.DAL.Mappers;
using InformationSystem.DAL.Repository;
using InformationSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using InformationSystem.BL.Mappers.Interfaces;

namespace InformationSystem.BL.Facades;

public abstract class
    FacadeBase<TEntity, TListModel, TDetailModel, TEntityMapper>(
        IUnitOfWorkFactory unitOfWorkFactory,
        IModelMapper<TEntity, TListModel, TDetailModel> modelModelMapper)
    : IFacade<TEntity, TListModel, TDetailModel>
    where TEntity : class, IEntity
    where TListModel : IModel
    where TDetailModel : class, IModel
    where TEntityMapper : IEntityMapper<TEntity>, new()
{
    protected readonly IModelMapper<TEntity, TListModel, TDetailModel> ModelModelMapper = modelModelMapper;
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory = unitOfWorkFactory;

    protected virtual ICollection<string> IncludesNavigationPathDetail => new List<string>();

    public async Task DeleteAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        try
        {
            await uow.GetRepository<TEntity, TEntityMapper>().DeleteAsync(id).ConfigureAwait(false);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("Entity deletion failed.", e);
        }
    }

    public virtual async Task<TDetailModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<TEntity> query = uow.GetRepository<TEntity, TEntityMapper>().Get();

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        TEntity? entity = await query.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

        return entity is null
            ? null
            : ModelModelMapper.MapToDetailModel(entity);
    }

    public virtual async Task<IEnumerable<TListModel>> GetAsync()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        List<TEntity> entities = await uow
            .GetRepository<TEntity, TEntityMapper>()
            .Get()
            .ToListAsync().ConfigureAwait(false);

        return ModelModelMapper.MapToListModel(entities);
    }

    public virtual async Task<IEnumerable<TListModel>> GetAsync(string orderBy, bool desc)
    {
        string descStr = desc ? "desc" : "";

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        List<TEntity> entities = await uow
            .GetRepository<TEntity, TEntityMapper>()
            .Get()
            .OrderBy($"{orderBy} {descStr}")
            .ToListAsync().ConfigureAwait(false);

        return ModelModelMapper.MapToListModel(entities);
    }

    public virtual async Task<TDetailModel> SaveAsync(TDetailModel model)
    {
        TDetailModel result;

        GuardCollectionsAreNotSet(model);

        TEntity entity = ModelModelMapper.MapToEntity(model);

        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<TEntity> repository = uow.GetRepository<TEntity, TEntityMapper>();

        if (await repository.ExistsAsync(entity).ConfigureAwait(false))
        {
            TEntity updatedEntity = await repository.UpdateAsync(entity).ConfigureAwait(false);
            result = ModelModelMapper.MapToDetailModel(updatedEntity);
        }
        else
        {
            entity.Id = Guid.NewGuid();
            TEntity insertedEntity = repository.Insert(entity);
            result = ModelModelMapper.MapToDetailModel(insertedEntity);
        }

        await uow.CommitAsync().ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// This Guard ensures that there is a clear understanding of current infrastructure limitations.
    /// This version of BL/DAL infrastructure does not support insertion or update of adjacent entities.
    /// WARN: Does not guard navigation properties.
    /// </summary>
    /// <param name="model">Model to be inserted or updated</param>
    /// <exception cref="InvalidOperationException"></exception>
    private static void GuardCollectionsAreNotSet(TDetailModel model)
    {
        IEnumerable<PropertyInfo> collectionProperties = model
            .GetType()
            .GetProperties()
            .Where(i => typeof(ICollection).IsAssignableFrom(i.PropertyType));

        foreach (PropertyInfo collectionProperty in collectionProperties)
        {
            if (collectionProperty.GetValue(model) is ICollection { Count: > 0 })
            {
                throw new InvalidOperationException(
                    "Current BL and DAL infrastructure disallows insert or update of models with adjacent collections.");
            }
        }
    }
}
