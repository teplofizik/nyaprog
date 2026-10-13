using System.Collections.Generic;

namespace Programmer.Modules
{
    /// <summary>
    /// Описание параметра команды (схема). Пусто у legacy-команд.
    /// </summary>
    public sealed class CommandParameter
    {
        /// <summary>Имя параметра (как в {placeholder}).</summary>
        public string Name;

        /// <summary>Имя типа ввода (см. InputType.Type), если нужен типизированный ввод.</summary>
        public string Type;

        /// <summary>Обязателен ли параметр.</summary>
        public bool Required;

        /// <summary>Значение по умолчанию.</summary>
        public string Default;

        /// <summary>Описание для UI/документации.</summary>
        public string Description;

        public CommandParameter(string Name, string Type = null, bool Required = false,
                                string Default = null, string Description = null)
        {
            this.Name = Name;
            this.Type = Type;
            this.Required = Required;
            this.Default = Default;
            this.Description = Description;
        }
    }

    /// <summary>
    /// Метаданные команды: идентификатор, назначение, поддерживаемые архитектуры,
    /// схема параметров.
    /// </summary>
    public sealed class CommandDescriptor
    {
        /// <summary>Полный идентификатор, например "core.copy" или "stlink.stm32.wflash".</summary>
        public string Id;

        /// <summary>Человекочитаемое название.</summary>
        public string Title;

        /// <summary>Описание.</summary>
        public string Description;

        /// <summary>Поддерживаемые архитектуры (пусто — без ограничения).</summary>
        public string[] SupportedArch = new string[0];

        /// <summary>Теги.</summary>
        public string[] Tags = new string[0];

        /// <summary>Схема параметров.</summary>
        public IReadOnlyList<CommandParameter> Parameters = new CommandParameter[0];

        public CommandDescriptor(string Id)
        {
            this.Id = Id;
        }
    }
}
