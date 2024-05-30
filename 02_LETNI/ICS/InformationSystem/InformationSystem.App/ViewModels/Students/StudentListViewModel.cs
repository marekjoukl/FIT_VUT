using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InformationSystem.App.Messages.Student;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Students;

public partial class StudentListViewModel(
    IStudentFacade studentFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<StudentEditMessage>, IRecipient<StudentDeleteMessage>
{
    public IEnumerable<StudentListModel> Students { get; set; } = null!;

    public string? InputText { get; set; } = null;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Students = await studentFacade.GetAsync();
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
        => await navigationService.GoToAsync( "/Detail",
            new Dictionary<string, object?> { [nameof(StudentDetailViewModel.Id)] = id });

    [RelayCommand]
    private async Task FilterNameAsync(string? fullName)
    {
        if (fullName == null)
            return;

        string[] parts = fullName.Split();
        if (parts.Length == 1)
        {
            Students = await studentFacade.GetByNameOrSurnameAsync(fullName);
        }
        else if (parts.Length == 2 && (!string.IsNullOrWhiteSpace(parts[0]) || !string.IsNullOrWhiteSpace(parts[1])))
        {
            Students = await studentFacade.GetByFullNameAsync(fullName);
        }
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await navigationService.GoToAsync("/Edit");
    }

    [RelayCommand]
    private async Task SortByOption(string orderBy)
    {
        Students = await studentFacade.GetAsync(orderBy, OrderDirection);
        OrderDirection = !OrderDirection;
    }

    public async void Receive(StudentEditMessage message)
    {
        await LoadDataAsync();
    }

    public async void Receive(StudentDeleteMessage message)
    {
        await LoadDataAsync();
    }
}
