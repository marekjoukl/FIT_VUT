using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Mappers.Interfaces;

public interface IRatingModelMapper : IModelMapper<RatingEntity, RatingListModel, RatingDetailModel>
{
    RatingListModel MapToListModel(RatingDetailModel detailModel);
    RatingEntity MapToEntity(RatingDetailModel model, Guid activityId);
    void MapToExistingDetailModel(RatingDetailModel existingDetailModel, RatingListModel rating);
    RatingEntity MapToEntity(RatingListModel model, Guid activityId);
}
