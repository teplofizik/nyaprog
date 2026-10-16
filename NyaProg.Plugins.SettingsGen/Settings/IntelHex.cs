using System.Collections.Generic;

namespace NyaProg.Plugins.SettingsGen.Settings
{
    /// <summary>Генерация Intel HEX (аналог hexnyan).</summary>
    public static class IntelHex
    {
        public static string Build(byte[] Data, int Offset)
        {
            var Lines = new List<string>();

            if (Offset > 0xFFFF)
            {
                byte Cs = 6;
                Cs += (byte)((Offset >> 16) & 0xFF);
                Cs += (byte)((Offset >> 24) & 0xFF);
                Lines.Add(string.Format(":02000004{0:X4}{1:X2}", (Offset >> 16) & 0xFFFF, (byte)(0 - Cs)));
            }

            int Line = 0;
            while (true)
            {
                var L = Line_(Data, Line++, Offset);
                if (L == null) break;
                Lines.Add(L);
            }

            Lines.Add(":00000001FF");
            return string.Join("\r\n", Lines) + "\r\n";
        }

        private static string Line_(byte[] Data, int Line, int Offset)
        {
            const int Width = 16;
            int DataStart = (Line * Width) & 0xFFFF;
            int Start = (DataStart + Offset) & 0xFFFF;
            if (DataStart > Data.Length) return null;

            int End = DataStart + Width - 1;
            if (End >= Data.Length) End = Data.Length - 1;
            int Length = End - DataStart + 1;
            if (Length <= 0) return null;

            string Result = "";
            byte Checksum = 0;

            Checksum += (byte)(Length & 0xFF);
            Checksum += (byte)((Start >> 8) & 0xFF);
            Checksum += (byte)(Start & 0xFF);

            Result += string.Format(":{0:X2}{1:X4}{2:X2}", Length, Start, 0);

            for (int i = 0; i < Length; i++)
            {
                Checksum += Data[DataStart + i];
                Result += string.Format("{0:X2}", Data[DataStart + i]);
            }

            Result += string.Format("{0:X2}", (byte)(0 - Checksum));
            return Result;
        }

        /// <summary>Бинарный выход с паддингом 0xFF до offset.</summary>
        public static byte[] Binary(byte[] Data, int Offset)
        {
            if (Offset <= 0) return Data;

            var Res = new byte[Offset + Data.Length];
            for (int i = 0; i < Offset; i++) Res[i] = 0xFF;
            System.Array.Copy(Data, 0, Res, Offset, Data.Length);
            return Res;
        }
    }
}
