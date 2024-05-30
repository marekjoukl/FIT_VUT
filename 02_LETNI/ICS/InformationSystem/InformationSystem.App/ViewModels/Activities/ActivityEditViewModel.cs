using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InformationSystem.App.Messages.Activity;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.Common.Enums;

namespace InformationSystem.App.ViewModels.Activities;

[QueryProperty(nameof(Activity), nameof(Activity))]
public partial class ActivityEditViewModel(
    IActivityFacade activityFacade,
    ISubjectFacade subjectFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService)
        , IRecipient<RatingDeleteMessage>, IRecipient<RatingEditMessage> , IRecipient<RatingAddMessage>
{
    public ActivityDetailModel Activity { get; set; } = ActivityDetailModel.Empty;

    public List<ActivityRoom> Rooms { get; set; } = new((ActivityRoom[])Enum.GetValues(typeof(ActivityRoom)));

    public List<ActivityType> Type { get; set; } = new((ActivityType[])Enum.GetValues(typeof(ActivityType)));

    public ObservableCollection<SubjectListModel> Subjects { get; set; } = new();

    public SubjectListModel? SubjectSelected
    {
        get => Subjects.FirstOrDefault(s => s.Id == Activity.SubjectId);
        set => Activity.SubjectId = value?.Id ?? Guid.Empty;
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Subjects.Clear();
        var subjects = await subjectFacade.GetAsync();
        foreach (var subject in subjects)
        {
            Subjects.Add(subject);
        }
    }


    [RelayCommand]
    private async Task SaveAsync()
    {
        await activityFacade.SaveAsync(Activity with { Students = default!, Subject = default!});

        MessengerService.Send(new ActivityEditMessage { ActivityId = Activity.Id});

        navigationService.SendBackButtonPressed();
    }

    [RelayCommand]
    private async Task GoToActivityStudentEditAsync()
    {
        await navigationService.GoToAsync("/Students",
            new Dictionary<string, object?> { [nameof(ActivityStudentEditViewModel.Activity)] = Activity });
    }

    public async void Receive(RatingDeleteMessage message)
    {
        await ReloadDataAsync();
    }

    public async void Receive(RatingEditMessage message)
    {
        await ReloadDataAsync();
    }

    public async void Receive(RatingAddMessage message)
    {
        await ReloadDataAsync();
    }

    private async Task ReloadDataAsync()
    {
        Activity = await activityFacade.GetAsync(Activity.Id)
                 ?? ActivityDetailModel.Empty;
    }
}
