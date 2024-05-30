using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformationSystem.App.ViewModels.Subjects;

namespace InformationSystem.App.Views;

public partial class SubjectDetailView : ContentPageBase
{
    public SubjectDetailView(SubjectDetailViewModel viewModel): base(viewModel)
    {
        InitializeComponent();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        NameEntry.WidthRequest = width * 0.7;
        AbbreviationEntry.WidthRequest = width * 0.7;
    }
}

