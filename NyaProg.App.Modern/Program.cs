using System;

using Avalonia;

namespace NyaProg.App.Modern
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args) =>
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .LogToTrace();
    }
}
