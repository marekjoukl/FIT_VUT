using InformationSystem.App.ViewModels.Activities;
using InformationSystem.BL.Models;

namespace InformationSystem.App.Views;

public partial class ActivityDetailView : ContentPageBase
{
	public ActivityDetailView(ActivityDetailViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
