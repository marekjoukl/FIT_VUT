using InformationSystem.App.ViewModels.Students;

namespace InformationSystem.App.Views;

public partial class StudentDetailView : ContentPageBase
{
    public StudentDetailView(StudentDetailViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}

