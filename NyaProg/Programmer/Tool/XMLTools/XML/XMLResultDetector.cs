using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.XMLTools.XML
{
    public abstract class XMLResultDetector
    {
        protected virtual string Check(ExternalToolResult R) => null;

        public string Get(ExternalToolResult R)
        {
            try
            {
                return Check(R);
            }
            catch(Exception E)
            {
                Log.WriteLine(E.Message);
                return null;
            }
        }
    }
}
