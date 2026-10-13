using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Programmer.Modules
{
    /// <summary>
    /// Подстановка плейсхолдеров {name} в командной строке.
    /// Разрешение имени делегируется <paramref name="Resolve"/> (с учётом алиасов).
    /// </summary>
    public static class CommandTemplating
    {
        private static readonly Regex ParamPattern = new Regex(@"\{([^};]*)\}");

        public static string Process(string Template, Func<string, string> Resolve)
        {
            if (Template == null) return null;

            var Result = Template;
            var Names = GetNames(Result);

            while (Names.Count > 0)
            {
                foreach (var K in Names)
                {
                    var Value = Resolve(K);
                    if (Value == null)
                        throw new InvalidOperationException($"no argument for param {{{K}}}: {Template}");

                    Result = Result.Replace($"{{{K}}}", Value);
                }

                Names = GetNames(Result);
            }

            return Result;
        }

        private static List<string> GetNames(string Template)
        {
            var Res = new List<string>();
            foreach (Match M in ParamPattern.Matches(Template))
                Res.Add(M.Groups[1].Value);

            return Res;
        }
    }
}
