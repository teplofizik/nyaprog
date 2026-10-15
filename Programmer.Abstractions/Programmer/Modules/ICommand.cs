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

        /// <summary>Выполнить команду.</summary>
        CommandResult Execute(CommandContext Ctx);
    }
}
