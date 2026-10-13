using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.Toolset
{
    class ProgToolset : ToolSet
    {
        public ProgToolset()
        {

        }

        public override void Load(string Dir) => LoadXMLTools(Dir, false);
    }
}
