namespace InformationSystem.App.Messages.Student;

public record StudentEditMessage
{
    public required Guid StudentId { get; init; }
}
