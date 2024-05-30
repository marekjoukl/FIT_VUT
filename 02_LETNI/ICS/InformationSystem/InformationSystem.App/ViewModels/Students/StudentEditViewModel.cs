using CommunityToolkit.Mvvm.Input;
using InformationSystem.App.Messages;
using InformationSystem.App.Messages.Student;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Students;

[QueryProperty(nameof(Student), nameof(Student))]
public partial class StudentEditViewModel(
    IStudentFacade studentFacade,
    INavigationService navigationService,
    IMessengerService messengerService) : ViewModelBase(messengerService)
{
    public StudentDetailModel Student { get; init; } = StudentDetailModel.Empty;

    [RelayCommand]
    private async Task SaveAsync()
    {
        await studentFacade.SaveAsync(Student with{ Subjects = default! });

        MessengerService.Send(new StudentEditMessage { StudentId = Student.Id});

        navigationService.SendBackButtonPressed();
    }

    [RelayCommand]
    private async Task GoToStudentSubjectsEditAsync()
    {
        await navigationService.GoToAsync("/Subjects",
            new Dictionary<string, object?> { [nameof(StudentSubjectsEditViewModel.Student)] = Student });
    }
}
