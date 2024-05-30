
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InformationSystem.App.Messages;
using InformationSystem.App.Messages.Activity;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.Common.Enums;

namespace InformationSystem.App.ViewModels.Activities;

public partial class ActivityListViewModel(
    IActivityFacade activityFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<ActivityEditMessage>, IRecipient<ActivityDeleteMessage>
{
    public IEnumerable<ActivityListModel> Activities { get; set; } = null!;

    public string? InputText { get; set; } = null;


    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Activities = await activityFacade.GetAsync();
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    => await navigationService.GoToAsync<ActivityDetailViewModel>(
            new Dictionary<string, object?> { [nameof(ActivityDetailViewModel.Id)] =  id});

    [RelayCommand]
    private async Task SortByOption(string orderBy)
    {
        Activities = await activityFacade.GetAsync(orderBy, OrderDirection);
        OrderDirection = !OrderDirection;
    }

    [RelayCommand]
    private async Task FilterBySubjectAsync(string? subject)
    {
        if (subject != null)
        {
            Activities = await activityFacade.GetAsync(subject, null, false);
        }
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync("/Edit");
    }

    public async void Receive(ActivityEditMessage message)
    {
        await LoadDataAsync();
    }
    public async void Receive(ActivityDeleteMessage message)
    {
        await LoadDataAsync();
    }
}
