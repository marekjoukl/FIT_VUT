using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Facades.Interfaces;

public interface ISubjectFacade : IFacade<SubjectEntity, SubjectListModel, SubjectDetailModel>
{
    Task<IEnumerable<SubjectListModel>> FilterByAbbAsync(string abbreviation);
}
