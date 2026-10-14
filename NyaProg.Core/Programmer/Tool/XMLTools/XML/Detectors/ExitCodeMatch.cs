using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Platform;

namespace Programmer.Tool.XMLTools.XML.Detectors
{
    class ExitCodeMatch : XMLResultDetector
    {
        protected override string Check(ProcessResult R)
        {
            return (R.ExitCode != 0) ? R.Output : null;
        }
    }
}
