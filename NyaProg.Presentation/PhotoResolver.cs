using System.IO;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Разрешение пути к изображению (относительно каталога проекта).
    /// </summary>
    static class PhotoResolver
    {
        public static string Resolve(string Dir, string Photo)
        {
            if (string.IsNullOrEmpty(Photo)) return null;

            var P = Path.IsPathRooted(Photo) ? Photo : Path.Combine(Dir ?? "", Photo);

            return File.Exists(P) ? P : null;
        }
    }
}
