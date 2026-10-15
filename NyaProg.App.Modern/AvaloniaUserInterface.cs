using System.Threading.Tasks;

using Avalonia.Threading;

using Programmer.Platform;

namespace NyaProg.App.Modern
{
    /// <summary>
    /// Реализация IUserInterface поверх Avalonia. Команды выполняются в фоновом
    /// потоке, поэтому диалог показывается через UI-поток.
    /// </summary>
    public sealed class AvaloniaUserInterface : IUserInterface
    {
        public void Message(string Text, string Title = "Сообщение")
        {
            var Tcs = new TaskCompletionSource<object>();
            Dispatcher.UIThread.Post(async () =>
            {
                await Dialogs.MessageAsync(Text, Title);
                Tcs.TrySetResult(null);
            });
            Tcs.Task.GetAwaiter().GetResult();
        }

        public bool Question(string Text, string Title = "Вопрос")
        {
            var Tcs = new TaskCompletionSource<bool>();
            Dispatcher.UIThread.Post(async () =>
            {
                var R = await Dialogs.QuestionAsync(Text, Title);
                Tcs.TrySetResult(R);
            });
            return Tcs.Task.GetAwaiter().GetResult();
        }
    }
}
