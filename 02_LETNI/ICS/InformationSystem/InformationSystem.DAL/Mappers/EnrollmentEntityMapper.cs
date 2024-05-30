using InformationSystem.DAL.Entities;

namespace InformationSystem.DAL.Mappers;

public class EnrollmentEntityMapper : IEntityMapper<EnrollmentEntity>
{
    public void MapToExistingEntity(EnrollmentEntity existingEntity, EnrollmentEntity newEntity)
    {
        existingEntity.StudentId = newEntity.StudentId;
        existingEntity.SubjectId = newEntity.SubjectId;
    }
}
