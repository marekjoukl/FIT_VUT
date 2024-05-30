using CommunityToolkit.Mvvm.Input;
using InformationSystem.App.Services;
using InformationSystem.App.Shells;

namespace InformationSystem.App;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // MainPage = new NavigationPage(new MainView());
        MainPage = serviceProvider.GetRequiredService<AppShell>();
    }

}
