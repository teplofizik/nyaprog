using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Programmer.Platform;

namespace Programmer.Tool.XMLTools.XML.Detectors
{
    public class RegexMatch : XMLResultDetector
    {
        private string Pattern;

        public RegexMatch(string Mask)
        {
            this.Pattern = Mask;
        }

        protected override string Check(ProcessResult R)
        {
            var Match = Regex.Matches(R.Output ?? "", Pattern, RegexOptions.IgnoreCase);
            if (Match.Count == 0) return null;

            var Text = "";
            foreach (Match M in Match)
                Text += M.Value + "\n";

            return Text;
        }
    }
}
