using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InformationSystem.App.Messages.Subject;
using InformationSystem.App.Services;
using InformationSystem.App.ViewModels.Activities;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Subjects;

public partial class SubjectListViewModel(
    ISubjectFacade subjectFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<SubjectEditMessage>, IRecipient<SubjectDeleteMessage>
{
    public IEnumerable<SubjectListModel> Subjects { get; set; } = null!;
    public string? InputText { get; set; } = null;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Subjects = await subjectFacade.GetAsync();
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync("./Detail");
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await navigationService.GoToAsync("./Detail",
            new Dictionary<string, object?> { [nameof(SubjectDetailViewModel.Id)] = id });
    }


    [RelayCommand]
    private async Task FilterNameAsync(string? abbreviation)
    {
        if (abbreviation == null)
            return;

        Subjects = await subjectFacade.FilterByAbbAsync(abbreviation);
    }

    [RelayCommand]
    private async Task SortByOption(string orderBy)
    {
        Subjects = await subjectFacade.GetAsync(orderBy, OrderDirection);
        OrderDirection = !OrderDirection;
    }

    public async void Receive(SubjectEditMessage message)
        => await LoadDataAsync();

    public async void Receive(SubjectDeleteMessage message)
        => await LoadDataAsync();
}
