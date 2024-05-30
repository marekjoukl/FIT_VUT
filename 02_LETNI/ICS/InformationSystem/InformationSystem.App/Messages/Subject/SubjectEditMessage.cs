namespace InformationSystem.App.Messages.Subject;

public record SubjectEditMessage
{
    public required Guid SubjectId { get; set; }
}
