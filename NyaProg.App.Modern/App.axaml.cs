using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using NyaProg.App.Modern.ViewModels;

namespace NyaProg.App.Modern
{
    public partial class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime Desktop)
            {
                var Env = AppHost.CreateEnv();
                var Shell = new ShellViewModel(Env);

                Desktop.MainWindow = new MainWindow { DataContext = Shell };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
