using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Mappers.Interfaces;

public interface IEnrollmentModelMapper : IModelMapper<EnrollmentEntity, EnrollmentListModel, EnrollmentDetailModel>
{
    EnrollmentListModel MapToListModel(EnrollmentDetailModel detailModel);
    EnrollmentEntity MapToEntity(EnrollmentDetailModel model, Guid recipeId);
    void MapToExistingDetailModel(EnrollmentDetailModel existingDetailModel, EnrollmentListModel enrollment);
    EnrollmentEntity MapToEntity(EnrollmentListModel model, Guid recipeId);
}
