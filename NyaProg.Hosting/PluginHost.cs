using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Programmer;
using Programmer.Environment;
using Programmer.Modules;

namespace NyaProg.Hosting
{
    /// <summary>
    /// Обнаружение и загрузка DLL-плагинов (модулей <see cref="IModule"/>).
    /// </summary>
    public static class PluginHost
    {
        /// <summary>Сборки хоста, которые не загружаем как плагины.</summary>
        private static readonly HashSet<string> Skip = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "NyaProg.Core", "Programmer.Abstractions", "NyaProg.Hosting", "NyaProg", "NyaProg.Cli"
        };

        private static bool ResolverInstalled;

        /// <summary>
        /// Найти модули в указанных каталогах и зарегистрировать их в окружении.
        /// </summary>
        public static void Load(ProgEnv Env, IEnumerable<string> Dirs)
        {
            if (Env == null) return;

            foreach (var M in Discover(Dirs))
            {
                Log.WriteLine($"Plugin module: {M.Name} ({M.Id}) v{M.Version}");
                Env.RegisterModule(M);
            }
        }

        /// <summary>
        /// Найти все модули в каталогах.
        /// </summary>
        public static IReadOnlyList<IModule> Discover(IEnumerable<string> Dirs)
        {
            var Res = new List<IModule>();
            if (Dirs == null) return Res;

            var Existing = new List<string>();
            foreach (var D in Dirs)
                if (Directory.Exists(D)) Existing.Add(D);

            InstallResolver(Existing);

            var Seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var Dir in Existing)
            {
                foreach (var File in Directory.GetFiles(Dir, "*.dll"))
                {
                    var Name = Path.GetFileNameWithoutExtension(File);
                    if (Skip.Contains(Name)) continue;
                    if (Name.StartsWith("System.") || Name.StartsWith("Microsoft.")) continue;
                    if (!Seen.Add(Path.GetFullPath(File))) continue;

                    Assembly Asm;
                    try { Asm = Assembly.LoadFrom(File); }
                    catch (Exception E) { Log.WriteLine($"Plugin load failed: {File}: {E.Message}"); continue; }

                    Res.AddRange(ModulesFrom(Asm));
                }
            }

            return Res;
        }

        private static IEnumerable<IModule> ModulesFrom(Assembly Asm)
        {
            var Res = new List<IModule>();

            Type[] Types;
            try { Types = Asm.GetTypes(); }
            catch (ReflectionTypeLoadException E) { Types = E.Types; }
            catch (Exception E) { Log.WriteLine($"Plugin types failed: {Asm.FullName}: {E.Message}"); return Res; }

            foreach (var T in Types)
            {
                if ((T == null) || T.IsAbstract || T.IsInterface) continue;
                if (!typeof(IModule).IsAssignableFrom(T)) continue;
                if (T.GetConstructor(Type.EmptyTypes) == null) continue;

                try { Res.Add((IModule)Activator.CreateInstance(T)); }
                catch (Exception E) { Log.WriteLine($"Module init failed: {T.FullName}: {E.Message}"); }
            }

            return Res;
        }

        /// <summary>
        /// Разрешать зависимости плагинов из их каталогов.
        /// </summary>
        private static void InstallResolver(List<string> Dirs)
        {
            if (ResolverInstalled) return;
            ResolverInstalled = true;

            AppDomain.CurrentDomain.AssemblyResolve += (S, E) =>
            {
                var Name = new AssemblyName(E.Name).Name + ".dll";

                foreach (var D in Dirs)
                {
                    var P = Path.Combine(D, Name);
                    if (File.Exists(P))
                    {
                        try { return Assembly.LoadFrom(P); } catch { }
                    }
                }

                return null;
            };
        }
    }
}
