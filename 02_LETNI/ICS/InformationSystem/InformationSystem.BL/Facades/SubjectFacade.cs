using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;
using InformationSystem.DAL.Mappers;
using InformationSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.BL.Facades;

public class SubjectFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    SubjectModelModelMapper modelModelMapper)
    :
        FacadeBase<SubjectEntity, SubjectListModel, SubjectDetailModel, SubjectEntityMapper>(
            unitOfWorkFactory, modelModelMapper), ISubjectFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] {$"{nameof(SubjectEntity.Activities)}"};

    public virtual async Task<IEnumerable<SubjectListModel>> FilterByAbbAsync(string abbreviation)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        List<SubjectEntity> entities = await uow
            .GetRepository<SubjectEntity, SubjectEntityMapper>()
            .Get()
            .Where(e => e.Abbreviation.Contains(abbreviation))
            .ToListAsync().ConfigureAwait(false);

        return modelModelMapper.MapToListModel(entities);
    }
}

