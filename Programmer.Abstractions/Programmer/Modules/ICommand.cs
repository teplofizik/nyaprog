namespace Programmer.Modules
{
    /// <summary>
    /// Единица работы: одно исполняемое действие.
    /// </summary>
    public interface ICommand
    {
        /// <summary>Полный идентификатор команды.</summary>
        string Id { get; }

        /// <summary>Метаданные и схема параметров.</summary>
        CommandDescriptor Descriptor { get; }

        /// <summary>
        /// Раскрывать ли {переменные} в значениях аргументов перед вызовом
        /// (для нативных команд — да; для legacy XML-инструментов — нет).
        /// </summary>
        bool ExpandArgs { get; }

        /// <summary>Выполнить команду.</summary>
        CommandResult Execute(CommandContext Ctx);
    }
}
