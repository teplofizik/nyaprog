using System;
using System.IO;

using Extension.Argument;

namespace Programmer.Platform
{
    /// <summary>
    /// Учёт временных файлов прогона. Команды tempfile/tempdir регистрируют пути
    /// в переменной окружения "$temp", а хост удаляет их по завершении прогона.
    /// </summary>
    public static class TempFiles
    {
        /// <summary>Ключ переменной со списком временных путей (по одному в строке).</summary>
        public const string ListKey = "$temp";

        /// <summary>Добавить путь к списку (возвращает новый список).</summary>
        public static string Append(string List, string Path)
        {
            if (string.IsNullOrEmpty(Path)) return List;
            return string.IsNullOrEmpty(List) ? Path : List + "\n" + Path;
        }

        /// <summary>Удалить файл или каталог (рекурсивно); ошибки игнорируются.</summary>
        public static void Delete(string Path)
        {
            if (string.IsNullOrEmpty(Path)) return;

            try
            {
                if (Directory.Exists(Path)) Directory.Delete(Path, true);
                else if (File.Exists(Path)) File.Delete(Path);
            }
            catch { }
        }

        /// <summary>
        /// Удалить все пути из переменной и очистить список.
        /// Возвращает количество обработанных путей.
        /// </summary>
        public static int Cleanup(ArgumentList Args)
        {
            if (Args == null) return 0;

            var List = Args.Get(ListKey);
            if (string.IsNullOrEmpty(List)) return 0;

            int Count = 0;
            foreach (var P in List.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var T = P.Trim();
                if (T.Length == 0) continue;

                Delete(T);
                Count++;
            }

            Args.Set(ListKey, "");
            return Count;
        }
    }
}
