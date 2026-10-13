using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Programmer.Tool
{
    /// <summary>
    /// Набор программаторов или команд
    /// </summary>
    public class ToolSet
    {
        public List<UniTool> Tools = new List<UniTool>();
        
        public virtual void Load(string Dir)
        {

        }

        /// <summary>
        /// Загрузить наборы команд для внешней CLI-программы
        /// </summary>
        /// <param name="Dir"></param>
        protected void LoadXMLTools(string Dir, bool Custom)
        {
            if(Directory.Exists(Dir))
            { 
                var Files = Directory.GetFiles(Dir, "*.etool");

                foreach (var F in Files)
                {
                    var T = new XMLTools.TlBaseXML(F, Custom);

                    if (T.Valid())
                        Tools.Add(T);
                    else
                        Log.WriteLine($"{T.Name} is not valid:");
                }
            }
            else
                Log.WriteLine($"Not found 'Tools' dir: {Dir}");
        }
    }
}
