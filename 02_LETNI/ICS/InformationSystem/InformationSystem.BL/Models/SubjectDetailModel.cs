// TODO dont know if this model is needed

using System.Collections.ObjectModel;

namespace InformationSystem.BL.Models;

public record SubjectDetailModel : ModelBase
{
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }

    // public ObservableCollection<ActivityListModel> Activities { get; init; } = new();

    public static SubjectDetailModel Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        Abbreviation = string.Empty
    };
}
