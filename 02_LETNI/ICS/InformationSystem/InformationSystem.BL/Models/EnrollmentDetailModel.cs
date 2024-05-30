namespace InformationSystem.BL.Models;

public record EnrollmentDetailModel : ModelBase
{
    // public required Guid StudentId { get; set; }
    // public required string StudentName { get; set; }
    // public required string StudentSurname { get; set; }

    public required Guid SubjectId { get; set; }
    public required string SubjectName { get; set; }
    public required string SubjectAbbreviation { get; set; }

    public static EnrollmentDetailModel Empty => new()
    {
        Id = Guid.NewGuid(),
        // StudentId = Guid.Empty,
        // StudentName = string.Empty,
        // StudentSurname = string.Empty,
        SubjectId = Guid.Empty,
        SubjectName = string.Empty,
        SubjectAbbreviation = string.Empty,
    };
}
