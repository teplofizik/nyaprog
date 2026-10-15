using System;
using System.Globalization;
using System.IO;

using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace NyaProg.App.Modern.Converters
{
    /// <summary>
    /// Путь к изображению → Bitmap (null, если файла нет).
    /// </summary>
    public sealed class PathToBitmapConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var Path = Value as string;

            if (string.IsNullOrEmpty(Path) || !File.Exists(Path)) return null;

            try { return new Bitmap(Path); }
            catch { return null; }
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) =>
            throw new NotSupportedException();
    }
}
