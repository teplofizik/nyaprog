using Programmer.XMLFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Options.XML
{
    static class XMLLoader
    {
        private static void LoadOption(Types.OptionList O, XmlLoad Base, XmlLoad X)
        {
            var I = new Types.OptionListItem(Base.GetAttribute("label"));
            I.Default = (Base.GetAttribute("default") != null);

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "value":
                        string N = X.GetAttribute("name");
                        string V = X.GetAttribute("val");

                        if (N != null) I.addValue(N, V);
                        break;
                }
            }

            X.Close();
            O.AddItem(I);
        }

        private static Types.OptionList LoadList(string Name, XmlLoad X)
        {
            var O = new Types.OptionList(Name);
            if (X.HasAttribute("id"))
                O.ID = X.GetAttribute("id");

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "option": LoadOption(O, X, X.GetSubtree()); break;
                }
            }

            X.Close();

            return O;
        }

        private static Types.OptionText LoadText(string Name, XmlLoad X)
        {
            var O = new Types.OptionText(Name);

            if (X.HasAttribute("id"))
                O.ID = X.GetAttribute("id");

            var Def = X.GetAttribute("default");
            if (Def != null) O.Default = Def;

            var N = X.GetAttribute("name");
            if (N != null) O.ParamName = N;

            return O;
        }

        private static Types.OptionHidden LoadHidden(string Name, XmlLoad X)
        {
            var O = new Types.OptionHidden(Name);

            if (X.HasAttribute("id"))
                O.ID = X.GetAttribute("id");

            var Def = X.GetAttribute("default");
            if (Def != null) O.Default = Def;

            var N = X.GetAttribute("name");
            if (N != null) O.ParamName = N;

            return O;
        }

        public static Option[] LoadOptions(XmlLoad X)
        {
            var Res = new List<Option>();
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "list": Res.Add(LoadList(X.GetAttribute("label"), X.GetSubtree())); break;
                    case "text": Res.Add(LoadText(X.GetAttribute("label"), X)); break;
                    case "hidden": Res.Add(LoadHidden(X.GetAttribute("label"), X)); break;
                }
            }

            X.Close();
            return Res.ToArray();
        }
    }
}
