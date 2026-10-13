namespace Programmer.Modules
{
    /// <summary>
    /// Роль инструмента. Раньше определялась каталогом загрузки (ProgTS/CustTS),
    /// теперь задаётся явно.
    /// </summary>
    public enum ToolKind
    {
        /// <summary>Программатор (прошивает устройства).</summary>
        Programmer,

        /// <summary>Внешняя утилита / конвертер / вспомогательная программа.</summary>
        Utility,

        /// <summary>Встроенный инструмент ядра (файлы, сообщения и т.п.).</summary>
        Internal
    }
}
