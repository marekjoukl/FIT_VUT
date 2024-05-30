using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformationSystem.App.ViewModels.Students;

namespace InformationSystem.App.Views;

public partial class StudentSubjectsEditView : ContentPageBase
{
    public StudentSubjectsEditView(StudentSubjectsEditViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}

