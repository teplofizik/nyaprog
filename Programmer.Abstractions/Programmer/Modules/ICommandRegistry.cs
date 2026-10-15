using System.Collections.Generic;

namespace Programmer.Modules
{
    /// <summary>
    /// Реестр инструментов и их команд. Заменяет перебор «первым совпавшим».
    /// </summary>
    public interface ICommandRegistry
    {
        /// <summary>Зарегистрировать инструмент.</summary>
        void Register(ITool Tool);

        /// <summary>Все зарегистрированные инструменты.</summary>
        IReadOnlyList<ITool> Tools { get; }

        /// <summary>Найти команду по идентификатору (первое совпадение).</summary>
        bool TryResolve(string CommandId, out ICommand Command);

        /// <summary>Найти команду, отдав приоритет указанному инструменту.</summary>
        bool TryResolve(string CommandId, ITool Preferred, out ICommand Command);
    }
}
