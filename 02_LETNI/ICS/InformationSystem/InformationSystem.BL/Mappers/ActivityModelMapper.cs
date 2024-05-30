using InformationSystem.BL.Mappers.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Mappers;

public class ActivityModelMapper(RatingModelMapper ratingModelMapper, SubjectModelModelMapper subjectModelModelMapper)
    : ModelMapperBase<ActivityEntity, ActivityListModel, ActivityDetailModel>,
        IActivityModelMapper
{
    public override ActivityListModel MapToListModel(ActivityEntity? entity)
        => entity is null
            ? ActivityListModel.Empty
            : new ActivityListModel
            {
                Id = entity.Id,
                Start = entity.Start,
                End = entity.End,
                Type = entity.Type,
                Room = entity.Room,
                SubjectId = entity.SubjectId,
                Subject = subjectModelModelMapper.MapToDetailModel(entity.Subject)
            };

    public override ActivityDetailModel MapToDetailModel(ActivityEntity? entity)
        => entity is null
            ? ActivityDetailModel.Empty
            : new ActivityDetailModel
            {
                Id = entity.Id,
                Start = entity.Start,
                End = entity.End,
                Type = entity.Type,
                Room = entity.Room,
                Description = entity.Description,
                SubjectId = entity.SubjectId,
                Subject = subjectModelModelMapper.MapToDetailModel(entity.Subject),
                Students = ratingModelMapper.MapToListModel(entity.Ratings).ToObservableCollection()
            };

    public ActivityListModel MapToListModel(ActivityDetailModel detailModel)
        => new()
        {
            Id = detailModel.Id,
            Start = detailModel.Start,
            End = detailModel.End,
            Type = detailModel.Type,
            Room = detailModel.Room,
            SubjectId = detailModel.SubjectId,
            Subject = detailModel.Subject
        };

    public void MapToExistingDetailModel(ActivityDetailModel existingDetailModel, ActivityListModel activity)
    {
        existingDetailModel.SubjectId = activity.SubjectId;
        existingDetailModel.Subject = activity.Subject;
    }

    public override ActivityEntity MapToEntity(ActivityDetailModel model)
        => new()
        {
            Id = model.Id,
            Start = model.Start,
            End = model.End,
            Type = model.Type,
            Room = model.Room,
            Description = model.Description,
            SubjectId = model.SubjectId,
            Subject = null!
            // Subject = subjectModelModelMapper.MapToEntity(model.Subject)
        };
}
