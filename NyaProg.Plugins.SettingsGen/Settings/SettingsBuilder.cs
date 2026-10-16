using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace NyaProg.Plugins.SettingsGen.Settings
{
    /// <summary>
    /// Сборка блока настроек: сначала все значения, затем вычисляемые поля
    /// (суммы/CRC/хэши) в порядке приоритета (order, затем позиция в файле).
    /// Источник вычислений — сам блок или внешний файл (source/offset/size).
    /// </summary>
    public static class SettingsBuilder
    {
        public static byte[] Build(List<Field> Fields, Func<string, string> Resolve,
                                   Dictionary<string, string> Overrides, Func<string, byte[]> ReadFile)
        {
            var Out = new List<byte>();
            var Computed = new List<ComputedField>();

            // Проход 1: раскладка (значения + места под вычисляемые)
            foreach (var F in Fields)
            {
                if (F.IsComputed)
                {
                    var Size = ComputedSize(F);
                    Computed.Add(new ComputedField { Field = F, Pos = Out.Count, Size = Size });
                    for (int i = 0; i < Size; i++) Out.Add(0);
                }
                else
                {
                    var B = BuildValue(F, Resolve, Overrides);
                    if (B != null) Out.AddRange(B);
                }
            }

            var Buf = Out.ToArray();

            // Проход 2: расчёт по приоритету (order, затем позиция)
            Computed.Sort((A, B) => (A.Field.Order != B.Field.Order)
                                  ? A.Field.Order.CompareTo(B.Field.Order)
                                  : A.Pos.CompareTo(B.Pos));

            foreach (var C in Computed)
            {
                var Data = SourceData(C, Buf, ReadFile);
                var Bytes = Compute(C.Field, Data);
                var N = Math.Min(Bytes.Length, C.Size);
                Array.Copy(Bytes, 0, Buf, C.Pos, N);
            }

            return Buf;
        }

        private sealed class ComputedField
        {
            public Field Field;
            public int Pos;
            public int Size;
        }

        private static int ComputedSize(Field F)
        {
            switch (F.Kind)
            {
                case FieldKind.Sum8:
                case FieldKind.Crc8: return 1;
                case FieldKind.Sum16:
                case FieldKind.Crc16: return 2;
                case FieldKind.Sum32:
                case FieldKind.Crc32: return 4;
                case FieldKind.Md5: return 16;
                case FieldKind.Sha1: return 20;
                case FieldKind.Sha256: return 32;
                case FieldKind.Len: return (F.Width > 0) ? F.Width : 2;
            }

            return 0;
        }

        private static byte[] SourceData(ComputedField C, byte[] Buf, Func<string, byte[]> ReadFile)
        {
            var F = C.Field;

            if (string.IsNullOrEmpty(F.Source))
            {
                // Сам блок настроек; по умолчанию — данные до этого поля
                int Start = (F.Offset > 0) ? F.Offset : 0;
                int End = (F.Size > 0) ? (Start + F.Size) : C.Pos;
                if (End > Buf.Length) End = Buf.Length;
                if (Start > End) Start = End;

                var Res = new byte[End - Start];
                Array.Copy(Buf, Start, Res, 0, Res.Length);
                return Res;
            }

            // Внешний файл
            var File = (ReadFile != null) ? ReadFile(F.Source) : null;
            if (File == null) return new byte[0];

            int S0 = (F.Offset > 0) ? F.Offset : 0;
            int E0 = (F.Size > 0) ? (S0 + F.Size) : File.Length;
            if (E0 > File.Length) E0 = File.Length;
            if (S0 > E0) S0 = E0;

            var R = new byte[E0 - S0];
            Array.Copy(File, S0, R, 0, R.Length);
            return R;
        }

        private static byte[] Compute(Field F, byte[] Data)
        {
            switch (F.Kind)
            {
                case FieldKind.Sum8: return new[] { (byte)(Checksum.Sum(Data) & 0xFF) };
                case FieldKind.Sum16: return Le(Checksum.Sum(Data), 2);
                case FieldKind.Sum32: return Le(Checksum.Sum(Data), 4);
                case FieldKind.Crc8: return Le(Crc.Compute(Data, 8, F.Poly, F.Init, F.XorOut, F.RefIn, F.RefOut), 1);
                case FieldKind.Crc16: return Le(Crc.Compute(Data, 16, F.Poly, F.Init, F.XorOut, F.RefIn, F.RefOut), 2);
                case FieldKind.Crc32: return Le(Crc.Compute(Data, 32, F.Poly, F.Init, F.XorOut, F.RefIn, F.RefOut), 4);
                case FieldKind.Md5: return MD5.Create().ComputeHash(Data);
                case FieldKind.Sha1: return SHA1.Create().ComputeHash(Data);
                case FieldKind.Sha256: return SHA256.Create().ComputeHash(Data);
                case FieldKind.Len: return Le((ulong)Data.Length, (F.Width > 0) ? F.Width : 2);
            }

            return new byte[0];
        }

        // *************** Значения ***************

        private static byte[] BuildValue(Field F, Func<string, string> Resolve, Dictionary<string, string> Overrides)
        {
            switch (F.Kind)
            {
                case FieldKind.U8: return new[] { (byte)Num(F, Overrides, Resolve) };
                case FieldKind.U16: return Le(Num(F, Overrides, Resolve), 2);
                case FieldKind.U32: return Le(Num(F, Overrides, Resolve), 4);
                case FieldKind.U48: return Le(Num(F, Overrides, Resolve), 6);
                case FieldKind.U64: return Le(Num(F, Overrides, Resolve), 8);
                case FieldKind.U8be: return new[] { (byte)Num(F, Overrides, Resolve) };
                case FieldKind.U16be: return Be(Num(F, Overrides, Resolve), 2);
                case FieldKind.U32be: return Be(Num(F, Overrides, Resolve), 4);
                case FieldKind.U48be: return Be(Num(F, Overrides, Resolve), 6);
                case FieldKind.U64be: return Be(Num(F, Overrides, Resolve), 8);

                case FieldKind.MAC: return Mac(Value(F, Overrides, Resolve));
                case FieldKind.IP4: return Ip(Value(F, Overrides, Resolve));
                case FieldKind.Str: return Str(Value(F, Overrides, Resolve), F.Width);
                case FieldKind.Hex: return Hex(Value(F, Overrides, Resolve), F.Width);
                case FieldKind.Pad: return Pad(F, Overrides, Resolve);
            }

            return null;
        }

        private static string Value(Field F, Dictionary<string, string> Overrides, Func<string, string> Resolve)
        {
            string V;
            if (!Overrides.TryGetValue(F.Name, out V)) V = F.Default;
            return ResolveValue(V, Resolve);
        }

        private static string ResolveValue(string V, Func<string, string> Resolve)
        {
            if (string.IsNullOrEmpty(V)) return V ?? "";

            switch (V)
            {
                case "{day}": return DateTime.Now.Day.ToString();
                case "{month}": return DateTime.Now.Month.ToString();
                case "{year}": return DateTime.Now.Year.ToString();
                case "{yearshort}": return (DateTime.Now.Year % 100).ToString();
                case "{user}": return Environment.UserName;
            }

            if (V.StartsWith("{") && V.EndsWith("}"))
            {
                var R = (Resolve != null) ? Resolve(V.Substring(1, V.Length - 2)) : null;
                if (R != null) return R;
            }

            return V;
        }

        /// <summary>Разбор числа: десятичное, 0x/0b/0o, ведущий 0 — восьмеричное.</summary>
        public static ulong ParseNumber(string V)
        {
            V = (V ?? "").Trim();
            if (V.Length == 0) return 0;

            try
            {
                if (V.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    return Convert.ToUInt64(V.Substring(2), 16);
                if (V.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
                    return Convert.ToUInt64(V.Substring(2), 2);
                if (V.StartsWith("0o", StringComparison.OrdinalIgnoreCase))
                    return Convert.ToUInt64(V.Substring(2), 8);
                if ((V.Length > 1) && (V[0] == '0') && IsOctal(V))
                    return Convert.ToUInt64(V, 8);

                return Convert.ToUInt64(V, 10);
            }
            catch { return 0; }
        }

        private static bool IsOctal(string V)
        {
            for (int i = 1; i < V.Length; i++)
                if ((V[i] < '0') || (V[i] > '7')) return false;

            return true;
        }

        private static ulong Num(Field F, Dictionary<string, string> Overrides, Func<string, string> Resolve)
            => ParseNumber(Value(F, Overrides, Resolve));

        private static byte[] Mac(string V)
        {
            var Res = new byte[6];
            var Parts = (V ?? "").Split(new[] { ':', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; (i < 6) && (i < Parts.Length); i++)
                byte.TryParse(Parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Res[i]);
            return Res;
        }

        private static byte[] Ip(string V)
        {
            IPAddress A;
            if (IPAddress.TryParse(V ?? "", out A)) return A.GetAddressBytes();
            return new byte[4];
        }

        private static byte[] Str(string V, int Width)
        {
            var Res = new byte[Width];
            var S = Encoding.ASCII.GetBytes(V ?? "");
            Array.Copy(S, Res, Math.Min(S.Length, Width));
            return Res;
        }

        private static byte[] Hex(string V, int Width)
        {
            var Clean = (V ?? "").Replace(" ", "").Replace(":", "");
            var Res = new byte[Width];
            for (int i = 0; (i < Width) && (i * 2 + 1 < Clean.Length); i++)
                byte.TryParse(Clean.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Res[i]);
            return Res;
        }

        private static byte[] Pad(Field F, Dictionary<string, string> Overrides, Func<string, string> Resolve)
        {
            var Res = new byte[F.Width];

            if (F.Values.Count == 0)
            {
                var B = (byte)Num(F, Overrides, Resolve);
                for (int i = 0; i < Res.Length; i++) Res[i] = B;
            }
            else
            {
                for (int i = 0; i < Res.Length; i++)
                    Res[i] = ParseByte(F.Values[i % F.Values.Count], Resolve);
            }

            return Res;
        }

        private static byte ParseByte(string V, Func<string, string> Resolve)
        {
            return (byte)ParseNumber(ResolveValue(V, Resolve));
        }

        private static byte[] Le(ulong Value, int Size)
        {
            var Res = new byte[Size];
            for (int i = 0; i < Size; i++) Res[i] = (byte)((Value >> (8 * i)) & 0xFF);
            return Res;
        }

        private static byte[] Be(ulong Value, int Size)
        {
            var Res = new byte[Size];
            for (int i = 0; i < Size; i++) Res[Size - 1 - i] = (byte)((Value >> (8 * i)) & 0xFF);
            return Res;
        }
    }
}
