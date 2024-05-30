using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;
using InformationSystem.DAL.Mappers;
using InformationSystem.DAL.Repository;
using InformationSystem.DAL.UnitOfWork;

namespace InformationSystem.BL.Facades;

public class RatingFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    RatingModelMapper ratingModelMapper)
    :
        FacadeBase<RatingEntity, RatingListModel, RatingDetailModel,
            RatingEntityMapper>(unitOfWorkFactory, ratingModelMapper), IRatingFacade
{
    public async Task SaveAsync(RatingListModel model, Guid activityId)
    {
        RatingEntity entity = ratingModelMapper.MapToEntity(model, activityId);

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<RatingEntity> repository =
            uow.GetRepository<RatingEntity, RatingEntityMapper>();

        if (await repository.ExistsAsync(entity))
        {
            await repository.UpdateAsync(entity);
            await uow.CommitAsync();
        }
    }

    public async Task SaveAsync(RatingDetailModel model, Guid activityId)
    {
        RatingEntity entity = ratingModelMapper.MapToEntity(model, activityId);

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<RatingEntity> repository =
            uow.GetRepository<RatingEntity, RatingEntityMapper>();

        repository.Insert(entity);
        await uow.CommitAsync();
    }
}
