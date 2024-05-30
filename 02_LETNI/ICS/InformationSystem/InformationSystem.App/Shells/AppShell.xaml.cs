using CommunityToolkit.Mvvm.Input;
using InformationSystem.App.Services;
using InformationSystem.App.ViewModels;
using InformationSystem.App.ViewModels.Activities;
using InformationSystem.App.ViewModels.Students;
using InformationSystem.App.ViewModels.Subjects;

namespace InformationSystem.App.Shells;

public partial class AppShell
{
    private readonly INavigationService _navigationService;

    public AppShell(INavigationService navigationService)
    {
        _navigationService = navigationService;

        InitializeComponent();
    }

    [RelayCommand]
    private async Task GoToEventsAsync()
        => await _navigationService.GoToAsync<ActivityListViewModel>();

    [RelayCommand]
    private async Task GoToStudentsAsync()
        => await _navigationService.GoToAsync<StudentListViewModel>();

    [RelayCommand]
    private async Task GoToSubjectsAsync()
        => await _navigationService.GoToAsync<SubjectListViewModel>();

}
