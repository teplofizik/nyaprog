using System;
using System.Collections.Generic;

namespace Programmer.Modules
{
    /// <summary>
    /// Точка входа плагина. Хост находит реализации IModule в загружаемых сборках
    /// и регистрирует предоставляемые ими инструменты.
    /// </summary>
    public interface IModule
    {
        /// <summary>Уникальный идентификатор модуля.</summary>
        string Id { get; }

        /// <summary>Название.</summary>
        string Name { get; }

        /// <summary>Версия.</summary>
        Version Version { get; }

        /// <summary>Инструменты, предоставляемые модулем.</summary>
        IEnumerable<ITool> GetTools();

        /// <summary>Инициализация модуля хостом.</summary>
        void Initialize(IModuleHost Host);
    }

    /// <summary>
    /// Необязательная база для модулей.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        public abstract string Id { get; }

        public virtual string Name => Id;

        public virtual Version Version => new Version(1, 0);

        public abstract IEnumerable<ITool> GetTools();

        public virtual void Initialize(IModuleHost Host) { }

        public override string ToString() => Name;
    }
}
