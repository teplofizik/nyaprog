using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NyaProg.App.Modern.Views
{
    public partial class FlashView : UserControl
    {
        public FlashView() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
