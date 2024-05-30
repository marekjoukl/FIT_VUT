using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;
using InformationSystem.DAL.Mappers;
using InformationSystem.DAL.Repository;
using InformationSystem.DAL.UnitOfWork;

namespace InformationSystem.BL.Facades;

public class EnrollmentFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    EnrollmentModelMapper enrollmentModelMapper)
    :
        FacadeBase<EnrollmentEntity, EnrollmentListModel, EnrollmentDetailModel,
            EnrollmentEntityMapper>(unitOfWorkFactory, enrollmentModelMapper), IEnrollmentFacade
{
    public async Task SaveAsync(EnrollmentListModel model, Guid studentId)
    {
        EnrollmentEntity entity = enrollmentModelMapper.MapToEntity(model, studentId);

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<EnrollmentEntity> repository =
            uow.GetRepository<EnrollmentEntity, EnrollmentEntityMapper>();

        if (await repository.ExistsAsync(entity))
        {
            await repository.UpdateAsync(entity);
            await uow.CommitAsync();
        }
    }

    public async Task SaveAsync(EnrollmentDetailModel model, Guid studentId)
    {
        EnrollmentEntity entity = enrollmentModelMapper.MapToEntity(model, studentId);

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<EnrollmentEntity> repository =
            uow.GetRepository<EnrollmentEntity, EnrollmentEntityMapper>();

        repository.Insert(entity);
        await uow.CommitAsync();
    }
}
