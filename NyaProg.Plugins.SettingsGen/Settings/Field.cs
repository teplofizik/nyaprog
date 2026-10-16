using System.Collections.Generic;

namespace NyaProg.Plugins.SettingsGen.Settings
{
    public enum FieldKind
    {
        U8, U16, U32, U48, U64,       // little-endian
        U8be, U16be, U32be, U48be, U64be, // big-endian
        MAC, IP4, Str, Hex, Pad,
        Sum8, Sum16, Sum32,
        Crc8, Crc16, Crc32,
        Md5, Sha1, Sha256,
        Len
    }

    /// <summary>Описание одного поля блока настроек.</summary>
    public sealed class Field
    {
        public FieldKind Kind;
        public string Name = "";
        public string Default = "";
        public string Comment = "";
        public int Width;                  // для Str/Hex/Pad/Len
        public List<string> Values = new List<string>();  // для Pad (<V>)

        // ---- Вычисляемые поля ----

        /// <summary>Приоритет расчёта: меньше — раньше (по умолчанию 0).</summary>
        public int Order;

        /// <summary>Внешний файл-источник данных (пусто — сам блок настроек).</summary>
        public string Source = "";

        /// <summary>Начальное смещение в источнике.</summary>
        public int Offset;

        /// <summary>Размер обрабатываемых данных (0 — до конца/до поля).</summary>
        public int Size;

        // ---- Параметры CRC (для Crc8/Crc16/Crc32) ----
        public ulong Init;
        public ulong Poly;
        public ulong XorOut;
        public bool RefIn;
        public bool RefOut;

        public bool IsComputed =>
            (Kind >= FieldKind.Sum8) && (Kind <= FieldKind.Len);
    }
}
