using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NyaProg.Plugins.HexConv
{
    /// <summary>Чтение и запись Intel HEX.</summary>
    public static class IntelHexCodec
    {
        // *************** Запись (bin → hex) ***************

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

        // *************** Чтение (hex → bin) ***************

        /// <summary>
        /// Разобрать Intel HEX. Возвращает байты от Base до максимального адреса;
        /// промежутки заполняются Fill. Base — адрес первого байта результата.
        /// </summary>
        public static byte[] Parse(string Text, int Offset, byte Fill, out int Base)
        {
            var Map = new Dictionary<int, byte>();
            int ExtBase = 0;
            int Min = int.MaxValue;
            int Max = int.MinValue;

            foreach (var Raw in Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var L = Raw.Trim();
                if ((L.Length == 0) || (L[0] != ':')) continue;

                var Bytes = FromHex(L.Substring(1));
                if (Bytes.Length < 5) continue;

                int Len = Bytes[0];
                int Addr = (Bytes[1] << 8) | Bytes[2];
                int Type = Bytes[3];

                if (Type == 0x01) break; // EOF

                if (Type == 0x04)
                {
                    if (Bytes.Length >= 6) ExtBase = ((Bytes[4] << 8) | Bytes[5]) << 16;
                    continue;
                }

                if (Type == 0x02)
                {
                    if (Bytes.Length >= 6) ExtBase = ((Bytes[4] << 8) | Bytes[5]) << 4;
                    continue;
                }

                if (Type != 0x00) continue;

                int A = ExtBase + Addr;
                for (int i = 0; i < Len; i++)
                {
                    if (4 + i >= Bytes.Length) break;

                    Map[A + i] = Bytes[4 + i];
                    if (A + i < Min) Min = A + i;
                    if (A + i > Max) Max = A + i;
                }
            }

            if (Min == int.MaxValue)
            {
                Base = 0;
                return new byte[0];
            }

            Base = (Offset > 0) ? Offset : Min;
            int Size = Max - Base + 1;
            if (Size < 0) Size = 0;

            var Res = new byte[Size];
            for (int i = 0; i < Size; i++) Res[i] = Fill;

            foreach (var P in Map)
            {
                int Idx = P.Key - Base;
                if ((Idx >= 0) && (Idx < Size)) Res[Idx] = P.Value;
            }

            return Res;
        }

        private static byte[] FromHex(string S)
        {
            var Res = new List<byte>();
            for (int i = 0; (i + 1) < S.Length; i += 2)
            {
                byte B;
                if (byte.TryParse(S.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out B))
                    Res.Add(B);
            }
            return Res.ToArray();
        }
    }
}
