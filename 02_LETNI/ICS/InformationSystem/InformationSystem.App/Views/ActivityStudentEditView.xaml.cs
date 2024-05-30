using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformationSystem.App.ViewModels.Activities;

namespace InformationSystem.App.Views;

public partial class ActivityStudentEditView : ContentPageBase
{
    public ActivityStudentEditView(ActivityStudentEditViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}

