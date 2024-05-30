using InformationSystem.App.ViewModels.Subjects;
using InformationSystem.BL.Models;

namespace InformationSystem.App.Views;

public partial class SubjectView : ContentPageBase
{
	public SubjectView(SubjectListViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
