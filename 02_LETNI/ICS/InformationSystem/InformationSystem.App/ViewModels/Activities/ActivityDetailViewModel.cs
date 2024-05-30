using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InformationSystem.App.Messages;
using InformationSystem.App.Messages.Activity;
using InformationSystem.App.Messages.Student;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Activities;


[QueryProperty(nameof(Id), nameof(Id))]
public partial class ActivityDetailViewModel(
        IActivityFacade activityFacade,
        INavigationService navigationService,
        IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<ActivityEditMessage>, IRecipient<RatingDeleteMessage>, IRecipient<RatingAddMessage>
{
    public Guid Id { get; set; }
    public ActivityDetailModel? Activity { get; set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Activity = await activityFacade.GetAsync(Id);
    }


    [RelayCommand]
    private async Task DeleteActivityAsync()
    {
        if (Activity is not null)
        {
            await activityFacade.DeleteAsync(Activity.Id);
            MessengerService.Send(new ActivityDeleteMessage());
            navigationService.SendBackButtonPressed();
        }
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (Activity != null)
        {
            await navigationService.GoToAsync("/Edit",
                new Dictionary<string, object?> { [nameof(ActivityEditViewModel.Activity)] = Activity with { } });
        }
    }

    [RelayCommand]
    public void SortByRatings()
    {
        if (Activity == null)
        {
            return;
        }

        var sortedRatings = Activity.Students.OrderBy(s => s.Points);
        Activity.Students = new ObservableCollection<RatingListModel>(sortedRatings);
    }

    public async void Receive(ActivityEditMessage message)
    {
        if (message.ActivityId == Activity?.Id)
        {
            await LoadDataAsync();
        }
    }

    public async void Receive(RatingDeleteMessage message)
    {
        await LoadDataAsync();
    }

    public async void Receive(RatingAddMessage message)
    {
        await LoadDataAsync();
    }
}
