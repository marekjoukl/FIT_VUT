using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using InformationSystem.App.Messages.Activity;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Activities;

[QueryProperty(nameof(Activity), nameof(Activity))]
public partial class ActivityStudentEditViewModel(
    IStudentFacade studentFacade,
    IRatingFacade ratingFacade,
    RatingModelMapper ratingModelMapper,
    IMessengerService messengerService) : ViewModelBase(messengerService)
{
    public ActivityDetailModel? Activity { get; set; }
    public ObservableCollection<StudentListModel> Students { get; set; } = new();
    public StudentListModel? StudentsSelected { get; set; }
    public RatingDetailModel? RatingNew { get; private set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Students.Clear();
        var students = await studentFacade.GetAsync();
        foreach (var student in students)
        {
            Students.Add(student);
            RatingNew = GetRatingNew();
        }
    }

    [RelayCommand]
    private async Task AddNewStudentToActivityAsync()
    {
        if (RatingNew is not null
            && StudentsSelected is not null
            && Activity is not null)
        {
            ratingModelMapper.MapToExistingDetailModel(RatingNew, StudentsSelected);

            await ratingFacade.SaveAsync(RatingNew, Activity.Id);
            Activity.Students.Add(ratingModelMapper.MapToListModel(RatingNew));

            RatingNew = GetRatingNew();

            MessengerService.Send(new RatingAddMessage());
        }
    }

    [RelayCommand]
    private async Task UpdateRatingAsync(RatingListModel? model)
    {
        if (model is not null
            && Activity is not null)
        {
            await ratingFacade.SaveAsync(model, Activity.Id);

            MessengerService.Send(new RatingEditMessage());
        }
    }

    [RelayCommand]
    private async Task RemoveSubjectAsync(RatingListModel model)
    {
        if (Activity is not null)
        {
            await ratingFacade.DeleteAsync(model.Id);
            Activity.Students.Remove(model);

            MessengerService.Send(new RatingDeleteMessage());
        }
    }

    private RatingDetailModel GetRatingNew()
    {
        var studentFirst = Students.First();
        return new RatingDetailModel
        {
            Id = Guid.NewGuid(),
            StudentId = studentFirst.Id,
            StudentName = studentFirst.Name,
            StudentSurname = studentFirst.Surname,
            Points = 0,
            Note = string.Empty
        };
    }
}
