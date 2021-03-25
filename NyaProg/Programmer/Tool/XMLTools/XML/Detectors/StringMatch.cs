using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.XMLTools.XML.Detectors
{
    public class StringMatch : XMLResultDetector
    {
        private string Mask;

        public StringMatch(string Mask)
        {
            this.Mask = Mask;
        }

        protected override string Check(ExternalToolResult R)
        {
            foreach (var L in R.Lines)
            {
                if (L.IndexOf(Mask) >= 0)
                    return L;
            }

            return null;
        }
    }
}
