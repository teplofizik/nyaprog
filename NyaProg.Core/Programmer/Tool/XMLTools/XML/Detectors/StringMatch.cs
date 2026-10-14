using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Platform;

namespace Programmer.Tool.XMLTools.XML.Detectors
{
    public class StringMatch : XMLResultDetector
    {
        private string Mask;

        public StringMatch(string Mask)
        {
            this.Mask = Mask;
        }

        protected override string Check(ProcessResult R)
        {
            var Lines = (R.Output ?? "").Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var L in Lines)
            {
                if (L.IndexOf(Mask) >= 0)
                    return L;
            }

            return null;
        }
    }
}
