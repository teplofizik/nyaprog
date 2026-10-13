using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.Toolset
{
    class CustomToolset : ToolSet
    {
        public override void Load(string Dir) => LoadXMLTools(Dir, true);

    }
}
