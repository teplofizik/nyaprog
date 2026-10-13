using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.XMLFile;

namespace Programmer.Environment.XML
{
    static class XMLLoader
    {
        public static bool Load(Preference P, string FileName)
        {
            XmlLoad X = new XmlLoad();
            if (!X.Load(FileName)) return false;

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "pref":
                        P.Set(X.GetAttribute("id"), X.GetAttribute("value"));
                        break;

                }
            }
            X.Close();

            return true;
        }
    }
}
