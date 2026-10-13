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

        /// <summary>Корневой каталог приложения ({root}).</summary>
        string Root { get; }

        /// <summary>Каталог Soft ({soft}).</summary>
        string Soft { get; }
    }
}
