using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InformationSystem.App.Messages.Subject;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Subjects;

[QueryProperty(nameof(Id), nameof(Id))]
public partial class SubjectDetailViewModel(
    ISubjectFacade subjectFacade,
    INavigationService navigationService,
    IMessengerService messengerService,
    IAlertService alertService)
    :ViewModelBase(messengerService),IRecipient<SubjectEditMessage>
{
    public Guid Id { get; set; }

    public SubjectDetailModel? Subject { get; private set; } = SubjectDetailModel.Empty;

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Subject = await subjectFacade.GetAsync(Id) ?? SubjectDetailModel.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await subjectFacade.SaveAsync(Subject);

        MessengerService.Send(new SubjectEditMessage() { SubjectId = Subject.Id});

        navigationService.SendBackButtonPressed();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Subject is not null)
        {
            try
            {
                await subjectFacade.DeleteAsync(Subject.Id);
                MessengerService.Send(new SubjectDeleteMessage());
                navigationService.SendBackButtonPressed();
            }
            catch (InvalidOperationException)
            {
                await alertService.DisplayAsync(DeleteErrorTitle, "Error while deleting Subject");
            }
        }
    }

    public async void Receive(SubjectEditMessage message)
    {
        if (message.SubjectId == Subject?.Id)
        {
            await LoadDataAsync();
        }
    }
}
