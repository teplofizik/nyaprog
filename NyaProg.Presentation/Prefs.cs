namespace NyaProg.Presentation
{
    /// <summary>
    /// Ключи предпочтений (хранятся в БД настроек).
    /// </summary>
    public static class Prefs
    {
        /// <summary>Программатор по умолчанию для типа контроллера.</summary>
        public static string ArchProgrammer(string Arch) => $"programmer.arch:{Arch}";
    }
}
