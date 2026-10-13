using System.Collections.Generic;

namespace Programmer.Modules
{
    /// <summary>
    /// Простой реестр команд: хранит инструменты и ищет команду по идентификатору.
    /// Приоритет отдаётся «предпочтительному» инструменту (например, выбранному
    /// пользователем программатору).
    /// </summary>
    sealed class CommandRegistry : ICommandRegistry
    {
        private readonly List<ITool> mTools = new List<ITool>();

        public void Register(ITool Tool)
        {
            if (Tool != null)
                mTools.Add(Tool);
        }

        public IReadOnlyList<ITool> Tools => mTools;

        public bool TryResolve(string CommandId, out ICommand Command)
        {
            Command = null;

            foreach (var T in mTools)
            {
                if (TryResolveIn(T, CommandId, out Command))
                    return true;
            }

            return false;
        }

        public bool TryResolve(string CommandId, ITool Preferred, out ICommand Command)
        {
            Command = null;

            // Сначала — выбранный инструмент (например, выбранный программатор).
            if ((Preferred != null) && TryResolveIn(Preferred, CommandId, out Command))
                return true;

            // Затем — утилиты и встроенные инструменты.
            // Прочие программаторы не подставляются неявно (сохранение прежнего поведения).
            foreach (var T in mTools)
            {
                if (T.Kind == ToolKind.Programmer)
                    continue;

                if (TryResolveIn(T, CommandId, out Command))
                    return true;
            }

            return false;
        }

        private static bool TryResolveIn(ITool T, string CommandId, out ICommand Command)
        {
            foreach (var C in T.GetCommands())
            {
                if ((C.Id == CommandId) || ((C.Descriptor != null) && (C.Descriptor.Id == CommandId)))
                {
                    Command = C;
                    return true;
                }
            }

            Command = null;
            return false;
        }
    }
}
