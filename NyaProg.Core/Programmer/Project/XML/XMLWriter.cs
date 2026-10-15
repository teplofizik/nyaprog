using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Programmer.Project.XML
{
    /// <summary>
    /// Запись прошивочного проекта в .nyap.
    /// </summary>
    public static class XMLWriter
    {
        public static void Save(Project P, string FileName)
        {
            var Dir = Path.GetDirectoryName(Path.GetFullPath(FileName));
            if (!string.IsNullOrEmpty(Dir) && !Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            var Settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                OmitXmlDeclaration = true,
                Encoding = new UTF8Encoding(false)
            };

            using (var W = XmlWriter.Create(FileName, Settings))
            {
                W.WriteStartElement("project");

                Tag(W, "name", P.Name);
                if (!string.IsNullOrEmpty(P.Date)) Tag(W, "date", P.Date);
                if (!string.IsNullOrEmpty(P.Description)) Tag(W, "description", P.Description);
                if (!string.IsNullOrEmpty(P.Photo)) Tag(W, "photo", P.Photo);
                if (!string.IsNullOrEmpty(P.Doc)) Tag(W, "doc", P.Doc);

                foreach (var A in P.Arch) Tag(W, "type", A);
                foreach (var T in P.Tags) Tag(W, "tag", T);

                if (!string.IsNullOrEmpty(P.Readme)) Tag(W, "readme", P.Readme);

                foreach (var T in P.Targets) WriteTarget(W, T);

                W.WriteEndElement();
            }
        }

        private static void Tag(XmlWriter W, string Name, string Value)
        {
            W.WriteStartElement(Name);
            W.WriteAttributeString("value", Value ?? "");
            W.WriteEndElement();
        }

        private static void WriteTarget(XmlWriter W, Target T)
        {
            W.WriteStartElement("target");

            Tag(W, "name", T.Name);
            if (!string.IsNullOrEmpty(T.Photo)) Tag(W, "photo", T.Photo);
            if (!string.IsNullOrEmpty(T.Doc)) Tag(W, "doc", T.Doc);
            if (!string.IsNullOrEmpty(T.Cathegory)) Tag(W, "cathegory", T.Cathegory);

            foreach (var A in T.Arch) Tag(W, "type", A);

            foreach (var K in T.Arguments.Keys)
                Tag(W, K, T.Arguments.Get(K));

            WriteSteps(W, T.Steps);

            W.WriteEndElement();
        }

        private static void WriteSteps(XmlWriter W, List<Step> Steps)
        {
            W.WriteStartElement("steps");
            foreach (var S in Steps) WriteStep(W, S);
            W.WriteEndElement();
        }

        private static void WriteStep(XmlWriter W, Step S)
        {
            var Cmd = S as Action;
            if (Cmd != null)
            {
                W.WriteStartElement(Cmd.Command);
                if (!string.IsNullOrEmpty(Cmd.Comment)) W.WriteAttributeString("comment", Cmd.Comment);
                foreach (var K in Cmd.Arguments.Keys) W.WriteAttributeString(K, Cmd.Arguments.Get(K));
                W.WriteEndElement();
                return;
            }

            var Set = S as SetStep;
            if (Set != null)
            {
                W.WriteStartElement("set");
                W.WriteAttributeString("name", Set.Name ?? "");
                W.WriteAttributeString("value", Set.Value ?? "");
                if (!string.IsNullOrEmpty(Set.Comment)) W.WriteAttributeString("comment", Set.Comment);
                W.WriteEndElement();
                return;
            }

            var If = S as IfStep;
            if (If != null)
            {
                W.WriteStartElement("if");
                W.WriteAttributeString("cond", If.Condition ?? "");
                if (!string.IsNullOrEmpty(If.Comment)) W.WriteAttributeString("comment", If.Comment);

                foreach (var C in If.Then) WriteStep(W, C);

                if (If.Else.Count > 0)
                {
                    W.WriteStartElement("else");
                    foreach (var C in If.Else) WriteStep(W, C);
                    W.WriteEndElement();
                }

                W.WriteEndElement();
            }
        }
    }
}
