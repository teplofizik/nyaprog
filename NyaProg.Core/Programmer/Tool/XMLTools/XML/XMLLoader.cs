using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Programmer.XMLFile;
using Programmer.Tool.XMLTools.XML.Detectors;

namespace Programmer.Tool.XMLTools.XML
{
    public static class XMLLoader
    {
        private static void LoadSupportedTypes(XMLTool T, XmlLoad X)
        {
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "type": T.ControllerTypes.Add(X.GetAttribute("value")); break;
                }
            }

            X.Close();
        }

        private static void LoadTags(XMLTool T, XmlLoad X)
        {
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "tag": T.ControllerTypes.Add(X.GetAttribute("value")); break;
                }
            }

            X.Close();
        }

        private static XMLToolAction LoadAction(XmlLoad X)
        {
            XMLToolAction A = new XMLToolAction();

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "command": A.Command = X.GetAttribute("value"); break;
                    case "toolpath": A.CustomToolPath = X.GetAttribute("value"); break;
                    case "params":
                        {
                            XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                var Name = Subtree.ElementName;
                                var Def = Subtree.GetAttribute("default") ?? "";

                                A.Defaults.Set(Name, Def);
                            }
                            Subtree.Close();
                        }
                        break;
                    case "error":
                        {
                            XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "string": A.ErrorMask.Add(new StringMatch(Subtree.GetAttribute("value"))); break;
                                    case "regex": A.ErrorMask.Add(new RegexMatch(Subtree.GetAttribute("value"))); break;
                                    case "exitcode": A.ErrorMask.Add(new ExitCodeMatch()); break;
                                }
                            }
                            Subtree.Close();
                        }
                        break;
                    case "warning":
                        {
                            XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "string": A.WarningMask.Add(new StringMatch(Subtree.GetAttribute("value"))); break;
                                    case "regex": A.WarningMask.Add(new RegexMatch(Subtree.GetAttribute("value"))); break;
                                    case "exitcode": A.WarningMask.Add(new ExitCodeMatch()); break;
                                }
                            }
                            Subtree.Close();
                        }
                        break;
                }
            }

            return A;
        }

        private static void LoadActions(XMLTool T, XmlLoad X, bool Custom)
        {
            while (X.Read())
            {
                XMLToolAction A = LoadAction(X.GetSubtree());
                A.Name = X.ElementName;

                T.Actions.Add(A);
            }

            X.Close();
        }

        private static void LoadOptions(XMLTool T, XmlLoad X)
        {
            var O = Options.XML.XMLLoader.LoadOptions(X);
            if(O != null) T.Options.AddRange(O);
        }

        private static string PreprocessPath(string Path)
        {
            string Root = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location));
            string Soft = System.IO.Path.Combine(Root, "Soft");

            Path = Path.Replace("{root}", Root);
            Path = Path.Replace("{soft}", Soft);

            return Path;
        }

        public static bool Load(string FileName, XMLTool Tool, bool Custom)
        {
            XmlLoad X = new XmlLoad();

            if (!X.Load(FileName)) return false;

            while (X.Read())
            {
                if (Custom)
                {
                    switch (X.ElementName)
                    {
                        case "name": Tool.ToolName = X.GetAttribute("value"); break;
                        case "path": Tool.ToolPath = PreprocessPath(X.GetAttribute("value")); break;
                        case "interpreter": Tool.Interpreter = X.GetAttribute("value"); break;
                        case "actions": LoadActions(Tool, X.GetSubtree(), true); break;
                        case "tags": LoadTags(Tool, X.GetSubtree()); break;
                    }
                }
                else
                {
                    switch (X.ElementName)
                    {
                        case "name": Tool.ToolName = X.GetAttribute("value"); break;
                        case "path": Tool.ToolPath = PreprocessPath(X.GetAttribute("value")); break;
                        case "interpreter": Tool.Interpreter = X.GetAttribute("value"); break;
                        case "supported": LoadSupportedTypes(Tool, X.GetSubtree()); break;
                        case "actions": LoadActions(Tool, X.GetSubtree(), false); break;
                        case "options": LoadOptions(Tool, X.GetSubtree()); break;
                        case "tags": LoadTags(Tool, X.GetSubtree()); break;
                    }
                }
            }

            X.Close();
            return true;
        }
    }
}
