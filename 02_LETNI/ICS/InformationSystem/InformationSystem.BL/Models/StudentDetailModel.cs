// TODO dont know if this model is needed

using System.Collections.ObjectModel;

namespace InformationSystem.BL.Models;

public record StudentDetailModel : ModelBase
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public string? PhotoUrl { get; set; }

    public ObservableCollection<EnrollmentListModel> Subjects { get; init; } = new();

    public static StudentDetailModel Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        Surname = string.Empty,
    };
}
