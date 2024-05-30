// TODO maybe add RatingsDetailModel class? dont know if necessary

namespace InformationSystem.BL.Models;

public record RatingListModel : ModelBase
{
    public float Points { get; set; }
    public string? Note { get; set; }

    public required Guid StudentId { get; set; }
    public required string StudentName { get; set; }
    public required string StudentSurname { get; set; }

    public static RatingListModel Empty => new()
    {
        Id = Guid.NewGuid(),
        StudentId = Guid.Empty,
        StudentName = string.Empty,
        StudentSurname = string.Empty,
    };
}
