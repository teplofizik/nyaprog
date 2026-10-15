using System.Collections.Generic;
using System.Linq;

using Extension.Argument;

namespace Programmer.Modules
{
    /// <summary>
    /// Итог выполнения команды.
    /// </summary>
    public enum CommandStatus
    {
        Ok,
        Warning,
        Error
    }

    /// <summary>
    /// Результат команды: статус, сообщение и, при необходимости,
    /// выходные переменные (для передачи данных между шагами скрипта).
    /// </summary>
    public sealed class CommandResult
    {
        public CommandStatus Status { get; }

        public string Message { get; }

        /// <summary>Выходные значения команды (пока пусто у legacy-команд).</summary>
        public ArgumentList Outputs { get; }

        /// <summary>Список элементов (для команд-поставщиков динамических опций).</summary>
        public IReadOnlyList<CommandItem> Items { get; }

        public CommandResult(CommandStatus Status, string Message, ArgumentList Outputs = null,
                             IReadOnlyList<CommandItem> Items = null)
        {
            this.Status = Status;
            this.Message = Message ?? "";
            this.Outputs = Outputs ?? new ArgumentList();
            this.Items = Items ?? new CommandItem[0];
        }

        public static CommandResult Ok(string Message = "") => new CommandResult(CommandStatus.Ok, Message);

        public static CommandResult Warning(string Message) => new CommandResult(CommandStatus.Warning, Message);

        public static CommandResult Error(string Message) => new CommandResult(CommandStatus.Error, Message);

        /// <summary>Успешный результат со списком элементов.</summary>
        public static CommandResult List(IEnumerable<CommandItem> Items, string Message = "") =>
            new CommandResult(CommandStatus.Ok, Message, null, Items?.ToArray());
    }
}
