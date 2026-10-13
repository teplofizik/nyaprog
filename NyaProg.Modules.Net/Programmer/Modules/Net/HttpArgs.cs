using System;
using System.Collections.Generic;

using Programmer.Modules;

namespace Programmer.Modules.Net
{
    /// <summary>
    /// Общие помощники сетевых команд: заголовки и разворачивание URL-шаблона.
    /// </summary>
    static class HttpArgs
    {
        /// <summary>Заголовки в формате "A: B; C: D".</summary>
        public static IReadOnlyDictionary<string, string> ParseHeaders(string Spec)
        {
            if (string.IsNullOrEmpty(Spec)) return null;

            var Res = new Dictionary<string, string>();

            foreach (var Part in Spec.Split(';'))
            {
                var P = Part.Trim();
                if (P.Length == 0) continue;

                var C = P.IndexOf(':');
                if (C <= 0) continue;

                Res[P.Substring(0, C).Trim()] = P.Substring(C + 1).Trim();
            }

            return Res;
        }

        /// <summary>Развернуть {переменные} в URL по окружению запуска.</summary>
        public static string ExpandUrl(CommandContext Ctx, string Url)
        {
            return CommandTemplating.Process(Url, N => Ctx.Host.ResolveArgument(N, Ctx.Args));
        }
    }
}
