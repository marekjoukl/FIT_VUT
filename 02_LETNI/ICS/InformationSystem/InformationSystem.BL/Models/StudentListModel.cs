namespace InformationSystem.BL.Models;

public record StudentListModel : ModelBase
{
    public required string Name { get; set; }
    public required string Surname { get; set; }

    public static StudentListModel Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        Surname = string.Empty
    };
}
