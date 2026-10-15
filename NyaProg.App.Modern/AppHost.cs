using NyaProg.Hosting;
using Programmer.Environment;

namespace NyaProg.App.Modern
{
    /// <summary>
    /// Загрузка окружения и плагинов для приложения.
    /// </summary>
    static class AppHost
    {
        public static ProgEnv CreateEnv()
        {
            var Env = new ProgEnv(new AvaloniaUserInterface());

            Env.Load("config.xml");
            Programmer.Log.LogFileName = Env.Conf.Log;

            PluginHost.Load(Env, Env.Conf.Plugins);

            return Env;
        }
    }
}
