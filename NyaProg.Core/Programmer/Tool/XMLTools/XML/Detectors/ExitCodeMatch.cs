using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.XMLTools.XML.Detectors
{
    class ExitCodeMatch : XMLResultDetector
    {
        protected override string Check(ExternalToolResult R)
        {
            return (R.ExitCode != 0) ? R.Result : null;
        }
    }
}
