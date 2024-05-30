using InformationSystem.BL.Mappers.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Mappers;

public class RatingModelMapper
    : ModelMapperBase<RatingEntity, RatingListModel, RatingDetailModel>,
        IRatingModelMapper
{
    public override RatingListModel MapToListModel(RatingEntity? entity)
        => entity?.Student is null
            ? RatingListModel.Empty
            : new RatingListModel
            {
                Id = entity.Id,
                Points = entity.Points,
                Note = entity.Note,

                StudentId = entity.StudentId,
                StudentName = entity.Student.Name,
                StudentSurname = entity.Student.Surname
            };

    public override RatingDetailModel MapToDetailModel(RatingEntity? entity)
        => entity?.Student is null
            ? RatingDetailModel.Empty
            : new RatingDetailModel
            {
                Id = entity.Id,
                Points = entity.Points,
                Note = entity.Note,

                StudentId = entity.StudentId,
                StudentName = entity.Student.Name,
                StudentSurname = entity.Student.Surname
            };

    public void MapToExistingDetailModel(RatingDetailModel existingDetailModel, RatingDetailModel rating)
    {
        existingDetailModel.StudentId = rating.StudentId;
        existingDetailModel.StudentName = rating.StudentName;
        existingDetailModel.StudentSurname = rating.StudentSurname;
    }

    public override RatingEntity MapToEntity(RatingDetailModel model)
        => throw new NotImplementedException("This method is unsupported. Use the other overload.");

    public RatingListModel MapToListModel(RatingDetailModel detailModel)
        => new()
        {
            Id = detailModel.Id,
            Note = detailModel.Note,
            Points = detailModel.Points,

            StudentId = detailModel.StudentId,
            StudentName = detailModel.StudentName,
            StudentSurname = detailModel.StudentSurname
        };

    public RatingEntity MapToEntity(RatingDetailModel model, Guid activityId)
        => new()
        {
            Id = model.Id,
            Points = model.Points,
            Note = model.Note,
            StudentId = model.StudentId,
            ActivityId = activityId,
            Activity = null!,
            Student = null!
        };

    public void MapToExistingDetailModel(RatingDetailModel existingDetailModel, RatingListModel rating)
    {
        existingDetailModel.StudentId = rating.StudentId;
        existingDetailModel.StudentName = rating.StudentName;
        existingDetailModel.StudentSurname = rating.StudentSurname;
        existingDetailModel.Note = rating.Note;
        existingDetailModel.Points = rating.Points;
    }

    public void MapToExistingDetailModel(RatingDetailModel existingDetailModel, StudentListModel student)
    {
        existingDetailModel.StudentId = student.Id;
        existingDetailModel.StudentName = student.Name;
        existingDetailModel.StudentSurname = student.Surname;
    }

    public RatingEntity MapToEntity(RatingListModel model, Guid activityId)
        => new()
        {
            Id = model.Id,
            Points = model.Points,
            Note = model.Note,
            StudentId = model.StudentId,
            ActivityId = activityId,
            Activity = null!,
            Student = null!
        };
}
