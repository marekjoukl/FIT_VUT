using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using InformationSystem.App.Messages.Student;
using InformationSystem.App.Services;
using InformationSystem.BL.Facades;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Mappers;
using InformationSystem.BL.Models;

namespace InformationSystem.App.ViewModels.Students;

[QueryProperty(nameof(Student), nameof(Student))]
public partial class StudentSubjectsEditViewModel(
    ISubjectFacade subjectFacade,
    IEnrollmentFacade enrollmentFacade,
    EnrollmentModelMapper enrollmentModelMapper,
    IMessengerService messengerService) : ViewModelBase(messengerService)
{
    public StudentDetailModel? Student { get; set; }
    public ObservableCollection<SubjectListModel> Subjects { get; set; } = new();
    public SubjectListModel? SubjectsSelected { get; set; }
    public EnrollmentDetailModel? EnrollmentNew { get; private set; }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        Subjects.Clear();
        var subjects = await subjectFacade.GetAsync();
        foreach (var subject in subjects)
        {
            Subjects.Add(subject);
            EnrollmentNew = GetEnrollmentNew();
        }
    }

    [RelayCommand]
    private async Task AddNewSubjectToStudentAsync()
    {
        if (EnrollmentNew is not null
            && SubjectsSelected is not null
            && Student is not null)
        {
            enrollmentModelMapper.MapToExistingDetailModel(EnrollmentNew, SubjectsSelected);

            await enrollmentFacade.SaveAsync(EnrollmentNew, Student.Id);
            Student.Subjects.Add(enrollmentModelMapper.MapToListModel(EnrollmentNew));

            EnrollmentNew = GetEnrollmentNew();

            MessengerService.Send(new StudentSubjectAddMessage());
        }
    }

    [RelayCommand]
    private async Task UpdateEnrollmentAsync(EnrollmentListModel? model)
    {
        if (model is not null
            && Student is not null)
        {
            await enrollmentFacade.SaveAsync(model, Student.Id);

            MessengerService.Send(new EnrollmentEditMessage());
        }
    }

    [RelayCommand]
    private async Task RemoveSubjectAsync(EnrollmentListModel model)
    {
        if (Student is not null)
        {
            await enrollmentFacade.DeleteAsync(model.Id);
            Student.Subjects.Remove(model);

            MessengerService.Send(new StudentSubjectDeleteMessage());
        }
    }

    private EnrollmentDetailModel GetEnrollmentNew()
    {
        var subjectFirst = Subjects.First();
        return new EnrollmentDetailModel
        {
            SubjectId = subjectFirst.Id,
            SubjectName = subjectFirst.Name,
            SubjectAbbreviation = subjectFirst.Abbreviation
        };
    }
}
