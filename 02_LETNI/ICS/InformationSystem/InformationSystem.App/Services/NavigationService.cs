using InformationSystem.App.Messages;
using InformationSystem.App.Models;
using InformationSystem.App.ViewModels;
using InformationSystem.App.ViewModels.Activities;
using InformationSystem.App.ViewModels.Students;
using InformationSystem.App.ViewModels.Subjects;
using InformationSystem.App.Views;
using InformationSystem.BL.Models;

namespace InformationSystem.App.Services;

public class NavigationService : INavigationService
{
    public IEnumerable<RouteModel> Routes { get; } = new List<RouteModel>
    {
        new("//Events", typeof(ActivityListView), typeof(ActivityListViewModel)),
        new("//Events/Detail", typeof(ActivityDetailView), typeof(ActivityDetailViewModel)),
        new("//Events/Edit", typeof(ActivityEditView), typeof(ActivityEditViewModel)),
        new("//Events/Detail/Edit/Students", typeof(ActivityStudentEditView), typeof(ActivityStudentEditViewModel)),

        new("//Students", typeof(StudentView), typeof(StudentListViewModel)),
        new("//Students/Detail", typeof(StudentDetailView), typeof(StudentDetailViewModel)),
        new("//Students/Edit", typeof(StudentEditView), typeof(StudentEditViewModel)),
        new("//Students/Detail/Edit/Subjects", typeof(StudentSubjectsEditView), typeof(StudentSubjectsEditViewModel)),

        new("//Subjects", typeof(SubjectView), typeof(SubjectListViewModel)),
        new("//Subjects/Detail", typeof(SubjectDetailView), typeof(SubjectDetailViewModel))
    };

    public async Task GoToAsync<TViewModel>()
        where TViewModel : IViewModel
    {
        var route = GetRouteByViewModel<TViewModel>();
        await Shell.Current.GoToAsync(route);
    }
    public async Task GoToAsync<TViewModel>(IDictionary<string, object?> parameters)
        where TViewModel : IViewModel
    {
        var route = GetRouteByViewModel<TViewModel>();
        await Shell.Current.GoToAsync(route, parameters);
    }

    public async Task GoToAsync(string route)
        => await Shell.Current.GoToAsync(route);

    public async Task GoToAsync(string route, IDictionary<string, object?> parameters)
        => await Shell.Current.GoToAsync(route, parameters);

    public bool SendBackButtonPressed()
        => Shell.Current.SendBackButtonPressed();

    private string GetRouteByViewModel<TViewModel>()
        where TViewModel : IViewModel
        => Routes.First(route => route.ViewModelType == typeof(TViewModel)).Route;
}
