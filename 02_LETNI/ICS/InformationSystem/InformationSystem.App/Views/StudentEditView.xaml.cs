using InformationSystem.App.ViewModels.Students;

namespace InformationSystem.App.Views;

public partial class StudentEditView : ContentPageBase
{
    public StudentEditView(StudentEditViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}

