using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Platform;

namespace Programmer.Tool.Toolset
{
    class CustomToolset : ToolSet
    {
        public CustomToolset(IUserInterface UI)
        {
            Tools.Add(new InternalTools.TlFileTools());
            Tools.Add(new InternalTools.TlMessages(UI));
            Tools.Add(new InternalTools.TlExternal());
        }

        public override void Load(string Dir) => LoadXMLTools(Dir, true);

    }
}
