using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Programmer.XMLFile
{
    class XmlLoad
    {
        private XmlReader F;

        public XmlLoad()
        {

        }

        public XmlLoad(XmlReader F)
        {
            this.F = F;
        }

        public bool Load(string FileName)
        {
            if (!File.Exists(FileName)) return false;
            if ((new FileInfo(FileName).Length) == 0)
            {
                File.Delete(FileName);
                return false;
            }

            F = XmlReader.Create(FileName);

            try
            {
                F.MoveToContent();
            }
            catch (Exception)
            {
                F.Close();

                File.Delete(FileName);
                return false;
            }

            return true;
        }

        public XmlLoad GetSubtree()
        {
            XmlReader R = F.ReadSubtree();

            // Read extern element
            R.Read();

            return new XmlLoad(R);
        }

        public void Close()
        {
            // F.Skip();
            F.Close();
        }

        public bool Read()
        {
            while (true)
            {
                try
                {
                    if (!F.Read()) return false;
                }
                catch (Exception E)
                {
                    Log.WriteLine("Error while reading xml: " + F.BaseURI);
                    Log.WriteLine(E.Message);
                    return false;
                }
                if (F.NodeType == XmlNodeType.Element) return true;
            }
        }

        public string ElementName
        {
            get { return F.Name; }
        }

        public bool HasAttribute(string Name)
        {
            return F.GetAttribute(Name) != null;
        }

        public string[] GetAttributeNames()
        {
            var Args = new List<string>();
            if (F.HasAttributes)
            {
                while (F.MoveToNextAttribute())
                {
                    Args.Add(F.Name);
                };
                F.MoveToElement();
            }

            return Args.ToArray();
        }

        public string GetAttribute(string Name)
        {
            return F.GetAttribute(Name);
        }

        public int GetIntAttribute(string Name)
        {
            var A = F.GetAttribute(Name);
            return (A != null) ? Convert.ToInt32(A) : 0;
        }

        public UInt64 GetUInt64Attribute(string Name)
        {
            return Convert.ToUInt64(F.GetAttribute(Name));
        }
    }
}
