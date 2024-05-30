using InformationSystem.BL.Mappers.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Mappers;

public class EnrollmentModelMapper
    : ModelMapperBase<EnrollmentEntity, EnrollmentListModel, EnrollmentDetailModel>,
        IEnrollmentModelMapper
{
    public override EnrollmentListModel MapToListModel(EnrollmentEntity? entity)
        => entity?.Subject is null
            ? EnrollmentListModel.Empty
            : new EnrollmentListModel
            {
                Id = entity.Id,
                SubjectId = entity.SubjectId,
                SubjectName = entity.Subject.Name,
                SubjectAbbreviation = entity.Subject.Abbreviation,
            };

    public override EnrollmentDetailModel MapToDetailModel(EnrollmentEntity? entity)
        => entity?.Subject is null
            ? EnrollmentDetailModel.Empty
            : new EnrollmentDetailModel
            {
                Id = entity.Id,
                SubjectId = entity.SubjectId,
                SubjectName = entity.Subject.Name,
                SubjectAbbreviation = entity.Subject.Abbreviation,
            };

    public EnrollmentListModel MapToListModel(EnrollmentDetailModel detailModel)
        => new()
        {
            Id = detailModel.Id,
            SubjectId = detailModel.SubjectId,
            SubjectName = detailModel.SubjectName,
            SubjectAbbreviation = detailModel.SubjectAbbreviation,
        };

    public void MapToExistingModel(EnrollmentDetailModel existingDetailModel, EnrollmentListModel enrollment)
    {
        existingDetailModel.SubjectId = enrollment.SubjectId;
        existingDetailModel.SubjectName = enrollment.SubjectName;
        existingDetailModel.SubjectAbbreviation = enrollment.SubjectAbbreviation;
    }

    public override EnrollmentEntity MapToEntity(EnrollmentDetailModel model)
        => throw new NotImplementedException("This method is unsupported. Use the other overload.");


    public EnrollmentEntity MapToEntity(EnrollmentDetailModel model, Guid studentId)
        => new()
        {
            Id = model.Id,
            StudentId = studentId,
            SubjectId = model.SubjectId,
            Student = null!,
            Subject = null!
        };

    public void MapToExistingDetailModel(EnrollmentDetailModel existingDetailModel, EnrollmentListModel enrollment)
    {
        existingDetailModel.SubjectId = enrollment.SubjectId;
        existingDetailModel.SubjectName = enrollment.SubjectName;
        existingDetailModel.SubjectAbbreviation = enrollment.SubjectAbbreviation;
    }

    public void MapToExistingDetailModel(EnrollmentDetailModel existingDetailModel, SubjectListModel subject)
    {
        existingDetailModel.SubjectId = subject.Id;
        existingDetailModel.SubjectName = subject.Name;
        existingDetailModel.SubjectAbbreviation = subject.Abbreviation;
    }

    public EnrollmentEntity MapToEntity(EnrollmentListModel model, Guid studentId)
        => new()
        {
            Id = model.Id,
            StudentId = studentId,
            SubjectId = model.SubjectId,
            Student = null!,
            Subject = null!
        };
}
