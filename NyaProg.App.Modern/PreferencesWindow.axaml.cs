using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using NyaProg.App.Modern.ViewModels;

namespace NyaProg.App.Modern
{
    public partial class PreferencesWindow : Window
    {
        public PreferencesWindow() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void OnSave(object Sender, RoutedEventArgs E) =>
            (DataContext as PreferencesViewModel)?.SaveCommand.Execute(null);

        private void OnClose(object Sender, RoutedEventArgs E) => Close();
    }
}
