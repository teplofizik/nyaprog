using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Platform;

namespace Programmer.Tool.XMLTools.XML
{
    public abstract class XMLResultDetector
    {
        protected virtual string Check(ProcessResult R) => null;

        public string Get(ProcessResult R)
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
