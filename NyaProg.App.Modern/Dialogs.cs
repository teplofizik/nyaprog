using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;

namespace NyaProg.App.Modern
{
    /// <summary>
    /// Простые модальные диалоги (сообщение/вопрос).
    /// </summary>
    static class Dialogs
    {
        private static Window Owner() =>
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        public static async Task MessageAsync(string Text, string Title)
        {
            var W = new Window
            {
                Title = Title,
                Width = 440,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var Ok = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Center, MinWidth = 90 };
            Ok.Click += (S, E) => W.Close();

            W.Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = Text, TextWrapping = TextWrapping.Wrap },
                    Ok
                }
            };

            var O = Owner();
            if (O != null) await W.ShowDialog(O); else W.Show();
        }

        public static async Task<bool> QuestionAsync(string Text, string Title)
        {
            var W = new Window
            {
                Title = Title,
                Width = 440,
                SizeToContent = SizeToContent.Height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var Yes = new Button { Content = "Да", MinWidth = 90, Margin = new Thickness(4) };
            var No = new Button { Content = "Нет", MinWidth = 90, Margin = new Thickness(4) };
            Yes.Click += (S, E) => W.Close(true);
            No.Click += (S, E) => W.Close(false);

            W.Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = Text, TextWrapping = TextWrapping.Wrap },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children = { Yes, No }
                    }
                }
            };

            var O = Owner();
            if (O == null) return false;

            return await W.ShowDialog<bool>(O);
        }
    }
}
