using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.Toolset
{
    class CustomToolset : ToolSet
    {
        public CustomToolset()
        {
            Tools.Add(new InternalTools.TlFileTools());
            Tools.Add(new InternalTools.TlMessages());
            Tools.Add(new InternalTools.TlExternal());
        }

        public override void Load(string Dir) => LoadXMLTools(Dir, true);

    }
}
