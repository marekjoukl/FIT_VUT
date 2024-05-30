using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Mappers.Interfaces;

public interface IActivityModelMapper : IModelMapper<ActivityEntity, ActivityListModel, ActivityDetailModel>
{
}
