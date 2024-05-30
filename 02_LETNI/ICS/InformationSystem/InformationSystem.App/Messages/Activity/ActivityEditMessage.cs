namespace InformationSystem.App.Messages.Activity;

public record ActivityEditMessage
{
    public required Guid ActivityId { get; init; }
}
