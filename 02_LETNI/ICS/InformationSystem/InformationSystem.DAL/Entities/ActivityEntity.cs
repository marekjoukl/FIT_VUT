using InformationSystem.Common.Enums;

namespace InformationSystem.DAL.Entities;

public record ActivityEntity : IEntity
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public ActivityRoom Room { get; set; }
    public ActivityType Type { get; set; }
    public string? Description { get; set; }
    public Guid SubjectId { get; set; }
    public required SubjectEntity Subject { get; set; }
    public ICollection<RatingEntity> Ratings { get; set; } = new List<RatingEntity>();
}
