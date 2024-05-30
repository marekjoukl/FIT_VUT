using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Facades.Interfaces;

public interface IEnrollmentFacade : IFacade<EnrollmentEntity, EnrollmentListModel, EnrollmentDetailModel>
{
    Task SaveAsync(EnrollmentDetailModel model, Guid studentId);

    Task SaveAsync(EnrollmentListModel model, Guid studentId);

    Task DeleteAsync(Guid id);
}
