using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InformationSystem.App.Messages.Student;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Students;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class StudentDetailViewModel(
    IStudentFacade studentFacade,
    INavigationService navigationService,
    IMessengerService messengerService)
    : ViewModelBase(messengerService), IRecipient<StudentEditMessage>, IRecipient<StudentSubjectAddMessage>,
        IRecipient<StudentSubjectDeleteMessage>
{
    public Guid Id { get; set; }
    public StudentDetailModel? Student { get; set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Student = await studentFacade.GetAsync(Id);
    }

    [RelayCommand]
    private async Task DeleteStudentAsync()
    {
        if (Student is not null)
        {
            await studentFacade.DeleteAsync(Student.Id);
            MessengerService.Send(new StudentDeleteMessage());
            navigationService.SendBackButtonPressed();
        }
    }

    [RelayCommand]
    private async Task GoToStudentEditAsync()
    {
        if (Student is not null)
        {
            await navigationService.GoToAsync("/Edit",
                new Dictionary<string, object?> { [nameof(StudentEditViewModel.Student)] = Student });
        }
    }

    public async void Receive(StudentEditMessage message)
    {
        if (message.StudentId == Student?.Id)
        {
            await LoadDataAsync();
        }
    }

    public async void Receive(StudentSubjectAddMessage addMessage)
    {
        await LoadDataAsync();
    }

    public async void Receive(StudentSubjectDeleteMessage message)
    {
        await LoadDataAsync();
    }
}
