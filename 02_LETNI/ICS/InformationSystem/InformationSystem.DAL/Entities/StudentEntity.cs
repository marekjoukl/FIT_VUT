namespace InformationSystem.DAL.Entities;

public record StudentEntity : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public string? PhotoUrl { get; set; }
    public ICollection<EnrollmentEntity> Subjects { get; init; } = new List<EnrollmentEntity>();
}
