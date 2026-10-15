using System.Collections.Generic;

using Programmer.XMLFile;

namespace Programmer.Project
{
    /// <summary>Контроллер в каталоге: название и тип (архитектура).</summary>
    public sealed class ControllerEntry
    {
        public string Name;
        public string Type;

        public ControllerEntry() { }

        public ControllerEntry(string Name, string Type)
        {
            this.Name = Name;
            this.Type = Type;
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Каталог контроллеров: список (название + тип) рядом с проектами.
    /// Тип используется для выбора программатора.
    /// </summary>
    public sealed class ControllerCatalog
    {
        public List<ControllerEntry> Entries = new List<ControllerEntry>();

        public static ControllerCatalog Load(string File)
        {
            var C = new ControllerCatalog();

            var X = new XmlLoad();
            if (!X.Load(File)) return C;

            while (X.Read())
            {
                if (X.ElementName == "controller")
                    C.Entries.Add(new ControllerEntry(X.GetAttribute("name"), X.GetAttribute("type")));
            }

            X.Close();
            return C;
        }

        public void Save(string File)
        {
            var S = new XmlSave(File);

            S.StartTag("catalog");
            foreach (var E in Entries)
            {
                S.StartTag("controller");
                S.Attribute("name", E.Name ?? "");
                S.Attribute("type", E.Type ?? "");
                S.EndTag();
            }
            S.EndTag();

            S.Close();
        }
    }
}
