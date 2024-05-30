using InformationSystem.Common.Enums;

namespace InformationSystem.BL.Models;

public record ActivityListModel : ModelBase
{
    public  DateTime Start { get; set; }
    public  DateTime End { get; set; }
    public  ActivityRoom Room { get; set; }
    public  ActivityType Type { get; set; }

    public required Guid SubjectId { get; set; }
    // public required string SubjectName { get; set; }
    //TODO CHECK IF WORKS
    public SubjectDetailModel Subject { get; set; } = new SubjectDetailModel(){Name = "", Abbreviation = ""};

    public static ActivityListModel Empty => new()
    {
        Id = Guid.Empty,
        SubjectId = Guid.Empty,
        Start = DateTime.MinValue,
        End = DateTime.MinValue,
        Room = ActivityRoom.Unknown,
    };
}
