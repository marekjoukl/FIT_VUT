using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using InformationSystem.Common.Enums;
namespace InformationSystem.BL.Models;

public record ActivityDetailModel : ModelBase
{
    private DateTime _start;
    private DateTime _end;

    public DateTime Start
    {
        get => _start;
        set
        {
            if (_start != value)
            {
                _start = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartDate));
                OnPropertyChanged(nameof(StartTime));
            }
        }
    }

    public DateTime End
    {
        get => _end;
        set
        {
            if (_end != value)
            {
                _end = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndDate));
                OnPropertyChanged(nameof(EndTime));
            }
        }
    }

    public DateTime StartDate
    {
        get => Start.Date;
        set
        {
            if (Start.Date != value)
            {
                Start = value.Add(Start.TimeOfDay);
            }
        }
    }

    public TimeSpan StartTime
    {
        get => Start.TimeOfDay;
        set
        {
            if (Start.TimeOfDay != value)
            {
                Start = Start.Date.Add(value);
            }
        }
    }

    public DateTime EndDate
    {
        get => End.Date;
        set
        {
            if (End.Date != value)
            {
                End = value.Add(End.TimeOfDay);
            }
        }
    }

    public TimeSpan EndTime
    {
        get => End.TimeOfDay;
        set
        {
            if (End.TimeOfDay != value)
            {
                End = End.Date.Add(value);
            }
        }
    }

    public ActivityRoom Room { get; set; }
    public ActivityType Type { get; set; }
    public string? Description { get; set; }
    public required Guid SubjectId { get; set; }
    public SubjectDetailModel Subject { get; set; } = new SubjectDetailModel { Name = "", Abbreviation = "" };

    public ObservableCollection<RatingListModel> Students { get; set; } = new();

    public static ActivityDetailModel Empty => new()
    {
        Id = Guid.Empty,
        SubjectId = Guid.Empty,
        Description = string.Empty,
        Start = DateTime.MinValue,
        End = DateTime.MinValue,
        Room = ActivityRoom.Unknown,
        Type = ActivityType.Unknown
    };
}
