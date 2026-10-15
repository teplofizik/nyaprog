using System;
using System.IO;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace NyaProg.App.Modern
{
    /// <summary>
    /// Показ документа: файл (текст/изображение) или URL; плюс текст-описание.
    /// </summary>
    public partial class DocumentWindow : Window
    {
        private string mExternal;

        public DocumentWindow() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        public void Setup(string Title, string Doc, string Dir, string Readme)
        {
            this.Title = string.IsNullOrEmpty(Title) ? "Документ" : Title;

            var Body = this.FindControl<StackPanel>("Body");
            var Open = this.FindControl<Button>("OpenExternal");

            if (!string.IsNullOrEmpty(Doc) && (Doc.StartsWith("http://") || Doc.StartsWith("https://")))
            {
                mExternal = Doc;
                Body.Children.Add(new TextBlock { Text = "Ссылка: " + Doc, TextWrapping = TextWrapping.Wrap });
                Open.IsVisible = true;
            }
            else if (!string.IsNullOrEmpty(Doc))
            {
                var P = Path.IsPathRooted(Doc) ? Doc : Path.Combine(Dir ?? "", Doc);

                if (File.Exists(P))
                {
                    mExternal = P;
                    Open.IsVisible = true;

                    var Ext = Path.GetExtension(P).ToLowerInvariant();
                    if ((Ext == ".png") || (Ext == ".jpg") || (Ext == ".jpeg") || (Ext == ".bmp") || (Ext == ".gif"))
                        Body.Children.Add(new Image { Source = new Bitmap(P), Stretch = Stretch.Uniform, MaxHeight = 500 });
                    else
                        Body.Children.Add(TextView(ReadFile(P)));
                }
                else
                    Body.Children.Add(new TextBlock { Text = "Файл не найден: " + P, TextWrapping = TextWrapping.Wrap });
            }
            else if (!string.IsNullOrEmpty(Readme))
                Body.Children.Add(TextView(Readme));
            else
                Body.Children.Add(new TextBlock { Text = "Документация не задана." });
        }

        private static string ReadFile(string Path)
        {
            try { return File.ReadAllText(Path); }
            catch (Exception E) { return "Не удалось прочитать файл: " + E.Message; }
        }

        private static Control TextView(string Text) => new TextBox
        {
            Text = Text,
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            BorderThickness = new Thickness(0),
            Background = null
        };

        private void OnClose(object Sender, RoutedEventArgs E) => Close();

        private async void OnOpenExternal(object Sender, RoutedEventArgs E)
        {
            if (string.IsNullOrEmpty(mExternal)) return;

            var Top = TopLevel.GetTopLevel(this);
            if (Top == null) return;

            if (mExternal.StartsWith("http"))
                await Top.Launcher.LaunchUriAsync(new Uri(mExternal));
            else
            {
                var F = await Top.StorageProvider.TryGetFileFromPathAsync(new Uri(Path.GetFullPath(mExternal)));
                if (F != null) await Top.Launcher.LaunchFileAsync(F);
            }
        }
    }
}
