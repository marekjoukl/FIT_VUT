using InformationSystem.App.ViewModels.Activities;
using InformationSystem.App.ViewModels.Subjects;

namespace InformationSystem.App.Views;

public partial class ActivityEditView : ContentPageBase
{
	public ActivityEditView(ActivityEditViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
