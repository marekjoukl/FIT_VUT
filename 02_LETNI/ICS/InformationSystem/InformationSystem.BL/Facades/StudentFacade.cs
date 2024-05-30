using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;
using InformationSystem.DAL.Mappers;
using InformationSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.BL.Facades;

public class StudentFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    StudentModelMapper modelMapper)
    :
        FacadeBase<StudentEntity, StudentListModel, StudentDetailModel, StudentEntityMapper>(
            unitOfWorkFactory, modelMapper), IStudentFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail =>
    new[] {$"{nameof(StudentEntity.Subjects)}.{nameof(EnrollmentEntity.Subject)}"};

    public virtual async Task<IEnumerable<StudentListModel>> GetByNameOrSurnameAsync(string searchedName)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        List<StudentEntity> entities = await uow
            .GetRepository<StudentEntity, StudentEntityMapper>()
            .Get()
            .Where(e => e.Surname.Contains(searchedName) || e.Name.Contains(searchedName))
            .ToListAsync().ConfigureAwait(false);

        return ModelModelMapper.MapToListModel(entities);
    }
    public virtual async Task<IEnumerable<StudentListModel>> GetByFullNameAsync(string fullName)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        List<StudentEntity> entities = await uow
            .GetRepository<StudentEntity, StudentEntityMapper>()
            .Get()
            .Where(e => (e.Name + " " + e.Surname).Contains(fullName))
            .ToListAsync().ConfigureAwait(false);

        return ModelModelMapper.MapToListModel(entities);
    }
}
