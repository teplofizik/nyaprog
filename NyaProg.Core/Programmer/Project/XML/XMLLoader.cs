using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.XMLFile;
using System.IO;

namespace Programmer.Project.XML
{
    static class XMLLoader
    {
        private static void LoadOptions(Target S, XmlLoad X)
        {
            var O = Options.XML.XMLLoader.LoadOptions(X);
            if (O != null) S.Options.AddRange(O);
        }

        private static void LoadOptions(Project P, XmlLoad X)
        {
            var O = Options.XML.XMLLoader.LoadOptions(X);
            if (O != null) P.Options.AddRange(O);
        }

        private static void LoadSteps(Target S, XmlLoad X)
        {
            while (X.Read())
            {
                var St = LoadStep(X);
                if (St != null) S.Steps.Add(St);
            }

            X.Close();
        }

        private static Step LoadStep(XmlLoad X)
        {
            var Name = X.ElementName;

            // Присвоение переменной
            if (Name == "set")
            {
                return new SetStep
                {
                    Name = X.GetAttribute("name"),
                    Value = X.GetAttribute("value"),
                    Comment = X.GetAttribute("comment")
                };
            }

            // Ветвление
            if (Name == "if")
            {
                var If = new IfStep
                {
                    Condition = X.GetAttribute("cond"),
                    Comment = X.GetAttribute("comment")
                };

                var Sub = X.GetSubtree();
                var Cur = If.Then;
                while (Sub.Read())
                {
                    if (Sub.ElementName == "else") { Cur = If.Else; continue; }
                    Cur.Add(LoadStep(Sub));
                }
                Sub.Close();

                return If;
            }

            // Обычная команда
            var A = new Action(Name);
            foreach (var Arg in X.GetAttributeNames())
            {
                switch (Arg)
                {
                    case "comment": A.Comment = X.GetAttribute(Arg); break;
                    default: A.Arguments.Set(Arg, X.GetAttribute(Arg)); break;
                }
            }

            return A;
        }

        private static string LoadFromFile(Project P, string FN)
        {
            if (File.Exists(FN))
                return File.ReadAllText(FN);

            var FNt = P.Dir + FN;
            if (File.Exists(FNt))
                return File.ReadAllText(FNt);

            FNt = Path.Combine(P.Dir, FN);
            if (File.Exists(FNt))
                return File.ReadAllText(FNt);

            return null;
        }
        
        private static void LoadTarget(Project P, XmlLoad X)
        {
            Target S = new Target();
            P.Targets.Add(S);

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "cathegory": S.Cathegory = X.GetAttribute("value"); break;
                    case "name": S.Name = X.GetAttribute("value"); break;
                    case "photo": S.Photo = X.GetAttribute("value"); break;
                    case "options": LoadOptions(S, X.GetSubtree()); break;
                    case "input":
                        {
                            var In = new InputParam();
                            if (X.HasAttribute("name")) In.Name = X.GetAttribute("name");
                            if (X.HasAttribute("value")) In.Type = X.GetAttribute("value");
                            if (X.HasAttribute("comment")) In.Comment = X.GetAttribute("comment");
                            if (X.HasAttribute("default")) In.Default = X.GetAttribute("default");
                            if (X.HasAttribute("id")) In.ID = X.GetAttribute("id");
                            if (X.HasAttribute("autoincrement"))
                                In.Autoincrement = X.GetAttribute("autoincrement") != "0";

                            S.Input.Add(In);
                        }
                        break;
                    case "steps": LoadSteps(S, X.GetSubtree()); break;
                    case "type": S.Arch.Add(X.GetAttribute("value")); break;
                    case "readme":
                        if (X.HasAttribute("file"))
                        {
                            var FN = X.GetAttribute("file");
                            S.Readme = LoadFromFile(P, FN);
                        }
                        else if (X.HasAttribute("value"))
                            S.Readme = X.GetAttribute("value");
                        break;
                    default: S.Arguments.Set(X.ElementName, X.GetAttribute("value")); break;
                }
            }
        }

        public static bool Load(Project P, string FileName)
        {
            XmlLoad X = new XmlLoad();
            if (!X.Load(FileName)) return false;

            P.File = FileName;
            P.Dir = Path.GetDirectoryName(FileName);

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "description": P.Description = X.GetAttribute("value"); break;
                    case "date": P.Date = X.GetAttribute("value"); break;
                    case "photo": P.Photo = X.GetAttribute("value"); break;
                    case "options": LoadOptions(P, X.GetSubtree()); break;
                    case "name": P.Name = X.GetAttribute("value"); break;
                    case "script":
                    case "target": LoadTarget(P, X.GetSubtree()); break;
                    case "tag": P.Tags.Add(X.GetAttribute("value")); break;
                    case "type": P.Arch.Add(X.GetAttribute("value")); break;
                    case "readme":
                        if (X.HasAttribute("file"))
                        {
                            var FN = X.GetAttribute("file");
                            P.Readme = LoadFromFile(P, FN);
                        }
                        else if (X.HasAttribute("value"))
                            P.Readme = X.GetAttribute("value");
                        break;
                }
            }

            X.Close();

            return true;
        }
    }
}
