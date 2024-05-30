using InformationSystem.DAL.Entities;

namespace InformationSystem.DAL.Mappers;

public class RatingEntityMapper : IEntityMapper<RatingEntity>
{
    public void MapToExistingEntity(RatingEntity existingEntity, RatingEntity newEntity)
    {
        existingEntity.Points = newEntity.Points;
        existingEntity.Note = newEntity.Note;
        existingEntity.ActivityId = newEntity.ActivityId;
        existingEntity.StudentId = newEntity.StudentId;
    }
}
