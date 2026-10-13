using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.XMLFile;

namespace Programmer.Environment.XML
{
    class XMLWriter
    {
        public static void Save(Preference P, string FileName)
        {
            var X = new XmlSave(FileName);

            X.StartXML("preferences");

            foreach (var ID in P.ID)
            {
                X.StartTag("pref");
                X.Attribute("id", ID);
                X.Attribute("value", P.Get(ID));
                X.EndTag();
            }
            X.EndXML();
            X.Close();
        }
    }
}
