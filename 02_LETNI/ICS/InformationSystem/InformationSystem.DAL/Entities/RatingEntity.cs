namespace InformationSystem.DAL.Entities;

public record RatingEntity : IEntity
{
    public Guid Id { get; set; }
    public float Points { get; set; }
    public string? Note { get; set; }
    public Guid ActivityId { get; set; }
    public required ActivityEntity Activity { get; set; }
    public Guid StudentId { get; set; }
    public required StudentEntity Student { get; set; }
}
