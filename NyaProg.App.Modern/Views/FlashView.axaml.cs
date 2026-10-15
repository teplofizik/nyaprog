using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using NyaProg.App.Modern.ViewModels;

namespace NyaProg.App.Modern.Views
{
    public partial class FlashView : UserControl
    {
        public FlashView() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void OnRefreshOption(object Sender, RoutedEventArgs E) =>
            ((Sender as Button)?.DataContext as OptionViewModel)?.Refresh?.Invoke();

        private void OnDocument(object Sender, RoutedEventArgs E)
        {
            var Vm = DataContext as FlashViewModel;
            if (Vm == null) return;

            var W = new DocumentWindow();
            W.Setup(Vm.Title, Vm.Doc, Vm.ProjectDir, Vm.Readme);
            W.ShowDialog(TopLevel.GetTopLevel(this) as Window);
        }
    }
}
