using Extension.Argument;
using Programmer.Platform;

namespace Programmer.Modules
{
    /// <summary>
    /// Сервисы, предоставляемые ядром модулям/командам.
    /// </summary>
    public interface IModuleHost
    {
        /// <summary>Реестр команд.</summary>
        ICommandRegistry Commands { get; }

        /// <summary>Взаимодействие с пользователем.</summary>
        IUserInterface UI { get; }

        /// <summary>Запуск внешних процессов.</summary>
        IProcessRunner ProcessRunner { get; }

        /// <summary>Сетевые запросы.</summary>
        IHttpFetcher Http { get; }

        /// <summary>Файловые операции.</summary>
        IFileSystem Files { get; }

        /// <summary>Корневой каталог приложения ({root}).</summary>
        string Root { get; }

        /// <summary>Каталог Soft ({soft}).</summary>
        string Soft { get; }

        /// <summary>
        /// Разрешить значение плейсхолдера с учётом алиасов (filename→value и т.п.).
        /// </summary>
        string ResolveArgument(string Name, ArgumentList Args);
    }
}
