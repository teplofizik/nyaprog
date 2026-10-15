using System.IO;

using Programmer.Environment;

namespace NyaProg.App.Modern
{
    /// <summary>Пути к данным приложения.</summary>
    static class AppPaths
    {
        /// <summary>Каталог контроллеров (рядом с проектами).</summary>
        public static string Catalog(ProgEnv Env)
        {
            var Root = (Env.Conf.Projects.Count > 0) ? Env.Conf.Projects[0] : "Projects";
            return Path.Combine(Root, "controllers.xml");
        }
    }
}
