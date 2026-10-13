using System;

namespace Programmer.Platform
{
    /// <summary>
    /// Взаимодействие с пользователем (сообщения, вопросы). Реализация — на стороне UI.
    /// Ядро не зависит от конкретной платформы (WinForms/консоль).
    /// </summary>
    public interface IUserInterface
    {
        void Message(string Text, string Title = "Сообщение");

        bool Question(string Text, string Title = "Вопрос");
    }

    /// <summary>
    /// Заглушка для headless-режима (CLI, тесты).
    /// </summary>
    public sealed class NullUserInterface : IUserInterface
    {
        public void Message(string Text, string Title = "Сообщение")
        {
            Console.WriteLine($"[{Title}] {Text}");
        }

        public bool Question(string Text, string Title = "Вопрос")
        {
            Console.WriteLine($"[{Title}] {Text} -> нет");
            return false;
        }
    }
}
