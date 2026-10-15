using Extension.Argument;

namespace Programmer.Modules
{
    /// <summary>
    /// Контекст выполнения команды: собранные аргументы/переменные, рабочий каталог
    /// и сервисы хоста.
    /// </summary>
    public sealed class CommandContext
    {
        /// <summary>Идентификатор выполняемой команды.</summary>
        public string CommandId;

        /// <summary>Разрешённые аргументы (плейсхолдеры {name}).</summary>
        public ArgumentList Args;

        /// <summary>Рабочий каталог ({dir}).</summary>
        public string WorkingDir;

        /// <summary>Сервисы хоста (реестр команд, UI, пути).</summary>
        public IModuleHost Host;

        public CommandContext(string CommandId, ArgumentList Args, string WorkingDir, IModuleHost Host)
        {
            this.CommandId = CommandId;
            this.Args = Args;
            this.WorkingDir = WorkingDir;
            this.Host = Host;
        }
    }
}
