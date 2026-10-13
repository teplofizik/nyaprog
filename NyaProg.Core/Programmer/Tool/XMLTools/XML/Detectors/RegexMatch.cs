using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Programmer.Tool.XMLTools.XML.Detectors
{
    public class RegexMatch : XMLResultDetector
    {
        private string Pattern;

        public RegexMatch(string Mask)
        {
            this.Pattern = Mask;
        }
        
        protected override string Check(ExternalToolResult R)
        {
            foreach (var L in R.Lines)
            {
                var match = Regex.Matches(R.Result, Pattern, RegexOptions.IgnoreCase);
                if (match.Count > 0)
                {
                    var Text = "";
                    foreach (Match M in match)
                        Text += M.Value + "\n";

                    return Text;
                }
            }

            return null;
        }
    }
}
