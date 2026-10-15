using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using NyaProg.App.Modern.ViewModels;

namespace NyaProg.App.Modern
{
    public partial class ControllersWindow : Window
    {
        public ControllersWindow() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void OnAdd(object Sender, RoutedEventArgs E) =>
            (DataContext as ControllersViewModel)?.Add();

        private void OnRemove(object Sender, RoutedEventArgs E)
        {
            var Vm = (Sender as Button)?.DataContext as ControllerEntryVM;
            (DataContext as ControllersViewModel)?.Remove(Vm);
        }

        private void OnSave(object Sender, RoutedEventArgs E) =>
            (DataContext as ControllersViewModel)?.Save();

        private void OnClose(object Sender, RoutedEventArgs E) => Close();
    }
}
