using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Programmer.XMLFile
{
    class XmlSave
    {
        private XmlWriter F;
        private bool Is4man = true;

        public XmlSave(string FileName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            {
                settings.Indent = true;
                settings.IndentChars = "    ";

                settings.NewLineChars = "\r\n";

                settings.OmitXmlDeclaration = true;
            }

            F = XmlWriter.Create(FileName, settings);
        }

        public void Close()
        {
            F.Close();
        }

        public void StartXML(string Element)
        {
            StartTag(Element);
            Attribute("date", DateTime.Now.ToString());
            Attribute("v", "1");
        }

        public void EndXML()
        {
            EndTag();
        }

        public void WriteTag(string Tag, string Attribute, string Value)
        {
            StartTag(Tag);
            this.Attribute(Attribute, Value);
            EndTag();
        }

        public void WriteTag(string Tag, string Attribute, int Value)
        {
            WriteTag(Tag, Attribute, Value.ToString());
        }

        public void WriteTag(string Tag, string Attribute, UInt64 Value)
        {
            WriteTag(Tag, Attribute, Value.ToString());
        }

        public void StartTag(string Tag)
        {
            F.WriteStartElement(Tag);
        }

        public void EndTag()
        {
            F.WriteEndElement();
        }

        public void Attribute(string Name, int Value)
        {
            F.WriteAttributeString(Name, Value.ToString());
        }

        public void Attribute(string Name, string Value)
        {
            F.WriteAttributeString(Name, Value);
        }
    }
}
