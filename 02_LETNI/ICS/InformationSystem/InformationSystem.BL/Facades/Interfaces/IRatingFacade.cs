using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Facades.Interfaces;

public interface IRatingFacade : IFacade<RatingEntity, RatingListModel, RatingDetailModel>
{
    Task SaveAsync(RatingDetailModel model, Guid activityId);

    Task SaveAsync(RatingListModel model, Guid activityId);

    Task DeleteAsync(Guid id);
}
