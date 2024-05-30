using InformationSystem.App.ViewModels.Activities;

namespace InformationSystem.App.Views;

public partial class ActivityListView : ContentPageBase
{
	public ActivityListView(ActivityListViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
