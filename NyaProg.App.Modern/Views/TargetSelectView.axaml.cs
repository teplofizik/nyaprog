using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using NyaProg.App.Modern.ViewModels;
using NyaProg.Presentation;

namespace NyaProg.App.Modern.Views
{
    public partial class TargetSelectView : UserControl
    {
        public TargetSelectView() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void OnCardClick(object Sender, RoutedEventArgs E)
        {
            var Card = (Sender as Button)?.DataContext as TargetCard;
            var Shell = DataContext as ShellViewModel;

            if ((Card != null) && (Shell != null)) Shell.OpenTarget(Card);
        }
    }
}
