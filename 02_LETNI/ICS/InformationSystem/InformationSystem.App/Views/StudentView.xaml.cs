using InformationSystem.App.ViewModels.Students;

namespace InformationSystem.App.Views;

public partial class StudentView : ContentPageBase
{
	public StudentView(StudentListViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
