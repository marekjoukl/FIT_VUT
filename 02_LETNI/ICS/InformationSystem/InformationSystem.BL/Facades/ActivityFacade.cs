using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;
using InformationSystem.DAL.Mappers;
using InformationSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace InformationSystem.BL.Facades;

public class ActivityFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    ActivityModelMapper modelMapper)
    :
        FacadeBase<ActivityEntity, ActivityListModel, ActivityDetailModel, ActivityEntityMapper>(
            unitOfWorkFactory, modelMapper), IActivityFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] {
            $"{nameof(ActivityEntity.Ratings)}.{nameof(RatingEntity.Student)}",
            $"{nameof(ActivityEntity.Subject)}"
        };

    public override async Task<IEnumerable<ActivityListModel>> GetAsync()

    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<ActivityEntity> query = uow.GetRepository<ActivityEntity, ActivityEntityMapper>().Get();

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        List<ActivityEntity> entities = await query.AsQueryable().ToListAsync().ConfigureAwait(false);

        return ModelModelMapper.MapToListModel(entities);
    }

    public override async Task<IEnumerable<ActivityListModel>> GetAsync(string orderBy, bool desc)
    {
        if (orderBy == "Subject")
            orderBy = "Subject.Abbreviation";
        string descStr = desc ? "desc" : "";

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<ActivityEntity> query = uow.GetRepository<ActivityEntity, ActivityEntityMapper>().Get();

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        List<ActivityEntity> entities = await query.AsQueryable()
            .OrderBy($"{orderBy} {descStr}")
            .ToListAsync().ConfigureAwait(false);

        return ModelModelMapper.MapToListModel(entities);
    }

    public virtual async Task<IEnumerable<ActivityListModel>> GetAsync(string subject, string? orderBy, bool desc)
    {
        string descStr = desc ? "desc" : "";
        if (orderBy == "Subject")
            orderBy = "Subject.Abbreviation";
        orderBy ??= "Id";

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<ActivityEntity> query = uow.GetRepository<ActivityEntity, ActivityEntityMapper>().Get();

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        List<ActivityEntity> entities = await query
            .AsQueryable()
            .Where(e => e.Subject.Abbreviation.Contains(subject))
            .OrderBy($"{orderBy} {descStr}")
            .ToListAsync().ConfigureAwait(false);

        return ModelModelMapper.MapToListModel(entities);
    }


}
