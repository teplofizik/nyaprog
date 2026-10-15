using Programmer.Options;

namespace Programmer.Modules
{
    /// <summary>
    /// Инструмент: контейнер команд, опций и поддерживаемых архитектур.
    /// Наследование от базового класса не обязательно — достаточно этого контракта.
    /// </summary>
    public interface ITool
    {
        /// <summary>Уникальный идентификатор инструмента.</summary>
        string Id { get; }

        /// <summary>Название.</summary>
        string Name { get; }

        /// <summary>Роль инструмента.</summary>
        ToolKind Kind { get; }

        /// <summary>Поддерживаемые архитектуры (пусто — без ограничения).</summary>
        string[] SupportedArch { get; }

        /// <summary>Теги.</summary>
        string[] Tags { get; }

        /// <summary>Опции, доступные пользователю.</summary>
        Option[] GetOptions();

        /// <summary>Команды инструмента.</summary>
        ICommand[] GetCommands();

        /// <summary>Поддерживает ли инструмент хотя бы одну из архитектур.</summary>
        bool SupportsArch(string[] Arch);
    }
}
