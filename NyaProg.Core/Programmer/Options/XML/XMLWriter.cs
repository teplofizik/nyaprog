using System.Collections.Generic;

using Programmer.Options.Types;

using XmlW = System.Xml.XmlWriter;

namespace Programmer.Options.XML
{
    /// <summary>
    /// Запись опций (проекта/цели) в .nyap. Формат совместим с <see cref="XMLLoader"/>.
    /// </summary>
    public static class XMLWriter
    {
        public static void Write(XmlW W, IEnumerable<Option> Options)
        {
            if (Options == null) return;

            var List = new List<Option>();
            foreach (var O in Options) if (O != null) List.Add(O);
            if (List.Count == 0) return;

            W.WriteStartElement("options");
            foreach (var O in List) WriteOption(W, O);
            W.WriteEndElement();
        }

        private static void Attr(XmlW W, string Name, string Value)
        {
            if (!string.IsNullOrEmpty(Value)) W.WriteAttributeString(Name, Value);
        }

        private static void WriteOption(XmlW W, Option O)
        {
            var L = O as OptionList;
            if (L != null) { WriteList(W, L); return; }

            var T = O as OptionText;
            if (T != null) { WriteText(W, T); return; }

            var H = O as OptionHidden;
            if (H != null) { WriteHidden(W, H); return; }

            var B = O as OptionBool;
            if (B != null) { WriteBool(W, B); return; }

            var D = O as OptionDynamic;
            if (D != null) { WriteDynamic(W, D); return; }
        }

        private static void WriteList(XmlW W, OptionList O)
        {
            W.WriteStartElement("list");
            Attr(W, "label", O.Name);
            Attr(W, "id", O.ID);

            foreach (var I in O.Items)
            {
                W.WriteStartElement("option");
                Attr(W, "label", I.Label);
                if (I.Default) Attr(W, "default", "1");

                foreach (var K in I.getNames())
                {
                    W.WriteStartElement("value");
                    W.WriteAttributeString("name", K);
                    W.WriteAttributeString("val", I.GetValue(K) ?? "");
                    W.WriteEndElement();
                }

                W.WriteEndElement();
            }

            W.WriteEndElement();
        }

        private static void WriteText(XmlW W, OptionText O)
        {
            W.WriteStartElement("text");
            Attr(W, "label", O.Name);
            Attr(W, "id", O.ID);
            Attr(W, "name", O.ParamName);
            Attr(W, "default", O.Default);
            if (O.Autoincrement) Attr(W, "autoincrement", "1");
            W.WriteEndElement();
        }

        private static void WriteHidden(XmlW W, OptionHidden O)
        {
            W.WriteStartElement("hidden");
            Attr(W, "label", O.Name);
            Attr(W, "id", O.ID);
            Attr(W, "name", O.ParamName);
            Attr(W, "default", O.Default);
            W.WriteEndElement();
        }

        private static void WriteBool(XmlW W, OptionBool O)
        {
            W.WriteStartElement("bool");
            Attr(W, "label", O.Name);
            Attr(W, "id", O.ID);
            Attr(W, "name", O.ParamName);
            Attr(W, "val", O.Value);
            if (O.Default) Attr(W, "default", "1");
            W.WriteEndElement();
        }

        private static void WriteDynamic(XmlW W, OptionDynamic O)
        {
            W.WriteStartElement("dynamic");
            Attr(W, "label", O.Name);
            Attr(W, "id", O.ID);
            Attr(W, "name", O.ParamName);
            Attr(W, "source", O.Source);
            Attr(W, "default", O.DefaultLabel);
            W.WriteEndElement();
        }
    }
}
