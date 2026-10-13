using System.Windows.Forms;
using Programmer.Platform;

namespace NyaProg
{
    /// <summary>
    /// Реализация IUserInterface поверх WinForms (в приложении, не в ядре).
    /// </summary>
    sealed class WinFormsUserInterface : IUserInterface
    {
        public void Message(string Text, string Title = "Сообщение")
        {
            MessageBox.Show(Text, Title);
        }

        public bool Question(string Text, string Title = "Вопрос")
        {
            return MessageBox.Show(Text, Title, MessageBoxButtons.YesNo) == DialogResult.Yes;
        }
    }
}
