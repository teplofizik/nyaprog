using System;
using System.Collections.Generic;
using System.Xml;

namespace NyaProg.Plugins.SettingsGen.Settings
{
    /// <summary>
    /// Загрузка описания блока настроек (<c>&lt;eeprom&gt;</c>) из XML.
    /// Имя элемента = тип поля. Значения: u8..u64, U8..U64, mac, ip, sN, xN, pN.
    /// Вычисляемые: sum8/sum16/sum32, crc8/crc16/crc32, md5, sha1, sha256, lenN.
    /// Атрибуты вычисляемых: order, source, offset, size, poly, init, xorout, refin, refout.
    /// </summary>
    public static class SettingsLoader
    {
        public static List<Field> Load(string XmlText)
        {
            var Res = new List<Field>();

            var Doc = new XmlDocument();
            Doc.LoadXml(XmlText);

            var Root = Doc.DocumentElement;
            if (Root == null) return Res;

            foreach (XmlNode Node in Root.ChildNodes)
            {
                if (Node.NodeType != XmlNodeType.Element) continue;

                var F = Parse(Node);
                if (F != null) Res.Add(F);
            }

            return Res;
        }

        private static Field Parse(XmlNode Node)
        {
            var Kind = ParseKind(Node.Name, out var Width);
            if (Kind == null) return null;

            var F = new Field
            {
                Kind = Kind.Value,
                Width = Width,
                Name = Attr(Node, "name"),
                Default = Attr(Node, "default"),
                Comment = Attr(Node, "comment"),
                Order = (int)AttrNum(Node, "order", 0),
                Source = Attr(Node, "source"),
                Offset = (int)AttrNum(Node, "offset", 0),
                Size = (int)AttrNum(Node, "size", 0)
            };

            if (Kind == FieldKind.Pad)
                foreach (XmlNode V in Node.ChildNodes)
                    if ((V.NodeType == XmlNodeType.Element) && (V.Name == "V"))
                        F.Values.Add(V.InnerText);

            ApplyCrcDefaults(F, Node);

            return F;
        }

        private static void ApplyCrcDefaults(Field F, XmlNode Node)
        {
            switch (F.Kind)
            {
                case FieldKind.Crc8:
                    F.Poly = AttrNum(Node, "poly", 0x07);
                    F.Init = AttrNum(Node, "init", 0x00);
                    break;
                case FieldKind.Crc16:
                    F.Poly = AttrNum(Node, "poly", 0x1021);
                    F.Init = AttrNum(Node, "init", 0xFFFF);
                    break;
                case FieldKind.Crc32:
                    F.Poly = AttrNum(Node, "poly", 0xEDB88320);
                    F.Init = AttrNum(Node, "init", 0xFFFFFFFF);
                    F.XorOut = AttrNum(Node, "xorout", 0xFFFFFFFF);
                    F.RefIn = AttrFlag(Node, "refin", true);
                    F.RefOut = AttrFlag(Node, "refout", true);
                    return;
            }

            F.XorOut = AttrNum(Node, "xorout", 0);
            F.RefIn = AttrFlag(Node, "refin", false);
            F.RefOut = AttrFlag(Node, "refout", false);
        }

        private static FieldKind? ParseKind(string Name, out int Width)
        {
            Width = 0;

            switch (Name)
            {
                case "u8": return FieldKind.U8;
                case "u16": return FieldKind.U16;
                case "u32": return FieldKind.U32;
                case "u48": return FieldKind.U48;
                case "u64": return FieldKind.U64;
                case "U8": return FieldKind.U8be;
                case "U16": return FieldKind.U16be;
                case "U32": return FieldKind.U32be;
                case "U48": return FieldKind.U48be;
                case "U64": return FieldKind.U64be;
                case "mac":
                case "MAC": return FieldKind.MAC;
                case "ip":
                case "IP4": return FieldKind.IP4;
                case "sum8": return FieldKind.Sum8;
                case "sum16": return FieldKind.Sum16;
                case "sum32": return FieldKind.Sum32;
                case "crc8": return FieldKind.Crc8;
                case "crc16": return FieldKind.Crc16;
                case "crc32": return FieldKind.Crc32;
                case "md5":
                case "MD5": return FieldKind.Md5;
                case "sha1":
                case "SHA1": return FieldKind.Sha1;
                case "sha256":
                case "SHA256": return FieldKind.Sha256;
            }

            if (Name.StartsWith("len", StringComparison.OrdinalIgnoreCase))
            {
                int W;
                Width = int.TryParse(Name.Substring(3), out W) ? W : 2;
                return FieldKind.Len;
            }

            // С параметром: sN / xN / pN
            var C = Name[0];
            int N;
            if (!int.TryParse(Name.Substring(1), out N)) return null;
            Width = N;

            switch (C)
            {
                case 's':
                case 'S': return FieldKind.Str;
                case 'x':
                case 'X': return FieldKind.Hex;
                case 'p':
                case 'P': return FieldKind.Pad;
            }

            return null;
        }

        private static string Attr(XmlNode Node, string Name)
        {
            var A = (Node.Attributes != null) ? Node.Attributes[Name] : null;
            return (A != null) ? A.Value : "";
        }

        private static ulong AttrNum(XmlNode Node, string Name, ulong Default)
        {
            var V = Attr(Node, Name);
            if (string.IsNullOrEmpty(V)) return Default;
            return SettingsBuilder.ParseNumber(V);
        }

        private static bool AttrFlag(XmlNode Node, string Name, bool Default)
        {
            var V = Attr(Node, Name);
            if (string.IsNullOrEmpty(V)) return Default;
            return (V != "0") && !string.Equals(V, "false", StringComparison.OrdinalIgnoreCase);
        }
    }
}
