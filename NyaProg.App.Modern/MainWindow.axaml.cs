using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using NyaProg.App.Modern.ViewModels;

namespace NyaProg.App.Modern
{
    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private async void OnPrefs(object Sender, RoutedEventArgs E)
        {
            var Shell = DataContext as ShellViewModel;
            if (Shell == null) return;

            var W = new PreferencesWindow { DataContext = new PreferencesViewModel(Shell.Session) };
            await W.ShowDialog(this);
        }

        private async void OnCreateProject(object Sender, RoutedEventArgs E)
        {
            var Shell = DataContext as ShellViewModel;
            if (Shell == null) return;

            var Vm = new ProjectEditorViewModel(Shell.Session, null);
            var W = new ProjectEditorWindow { DataContext = Vm };
            await W.ShowDialog(this);

            Shell.AfterEdit(Vm.Saved);
        }

        private async void OnEditProject(object Sender, RoutedEventArgs E)
        {
            var Shell = DataContext as ShellViewModel;
            if ((Shell == null) || (Shell.Project == null)) return;

            var Vm = new ProjectEditorViewModel(Shell.Session, Shell.Project.Project);
            var W = new ProjectEditorWindow { DataContext = Vm };
            await W.ShowDialog(this);

            Shell.AfterEdit(Vm.Saved);
        }
    }
}
