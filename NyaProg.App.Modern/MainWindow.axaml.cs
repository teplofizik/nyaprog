using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NyaProg.App.Modern
{
    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
