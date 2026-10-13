using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Programmer.Project
{
    class ProjLibrary
    {
        /// <summary>
        /// Список проектов
        /// </summary>
        public List<Project> Projects = new List<Project>();

        /// <summary>
        /// Загрузить проекты из указанной папки
        /// </summary>
        /// <param name="Dir"></param>
        public void Load(string Dir)
        {
            if (Directory.Exists(Dir))
            {
                // Получим все папки
                var Dirs = Directory.GetDirectories(Dir);

                foreach (var D in Dirs)
                {
                    var Files = Directory.GetFiles(D, "*.nyap");
                    foreach (var F in Files)
                    {
                        var P = new Project();
                        XML.XMLLoader.Load(P, F);

                        Projects.Add(P);
                    }
                }
            }
            else
                Log.WriteLine($"Not found 'Projects' dir: {Dir}");
        }
    }
}
