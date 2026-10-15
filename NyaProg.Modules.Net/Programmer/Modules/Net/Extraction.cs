using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

using Extension.Argument;

namespace Programmer.Modules.Net
{
    /// <summary>
    /// Разбор ответа в переменные. Спецификация — список правил через ';',
    /// каждое правило: name=тип:выражение.
    ///   name=text:               — всё тело как текст
    ///   name=regex:pattern       — первая группа (или всё совпадение)
    ///   name=json:$.a.b[0].c     — путь по JSON
    /// Если спецификация пуста — тело пишется в переменную body.
    /// </summary>
    static class Extraction
    {
        public static void Apply(string Spec, string Body, ArgumentList Outputs)
        {
            if (string.IsNullOrEmpty(Spec))
            {
                Outputs.Set("body", Body ?? "");
                return;
            }

            foreach (var Rule in Spec.Split(';'))
            {
                var R = Rule.Trim();
                if (R.Length == 0) continue;

                var Eq = R.IndexOf('=');
                if (Eq <= 0) continue;

                var Name = R.Substring(0, Eq).Trim();
                var Expr = R.Substring(Eq + 1).Trim();
                if (Name.Length == 0) continue;

                var Value = Evaluate(Expr, Body);
                if (Value != null) Outputs.Set(Name, Value);
            }
        }

        private static string Evaluate(string Expr, string Body)
        {
            var Colon = Expr.IndexOf(':');
            var Type = (Colon >= 0) ? Expr.Substring(0, Colon).Trim().ToLowerInvariant() : "text";
            var Arg = (Colon >= 0) ? Expr.Substring(Colon + 1) : Expr;

            switch (Type)
            {
                case "json": return JsonValue(Body, Arg);
                case "regex": return RegexValue(Body, Arg);
                case "text": return Body;
                default: return Body;
            }
        }

        private static string RegexValue(string Body, string Pattern)
        {
            if (string.IsNullOrEmpty(Pattern)) return Body;

            var M = Regex.Match(Body, Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!M.Success) return null;

            return (M.Groups.Count > 1) ? M.Groups[1].Value : M.Value;
        }

        private static string JsonValue(string Body, string Path)
        {
            using (var Doc = JsonDocument.Parse(Body))
                return JsonPath(Doc.RootElement, Path);
        }

        private static string JsonPath(JsonElement Root, string Path)
        {
            if (string.IsNullOrEmpty(Path)) return null;

            var Cur = Root;
            var I = Path.StartsWith("$") ? 1 : 0;

            while (I < Path.Length)
            {
                if (Path[I] == '.')
                {
                    I++;
                    var S = I;
                    while ((I < Path.Length) && (Path[I] != '.') && (Path[I] != '[')) I++;
                    var Name = Path.Substring(S, I - S);

                    if ((Cur.ValueKind != JsonValueKind.Object) || !Cur.TryGetProperty(Name, out var Next))
                        return null;

                    Cur = Next;
                }
                else if (Path[I] == '[')
                {
                    I++;
                    var S = I;
                    while ((I < Path.Length) && (Path[I] != ']')) I++;
                    if (!int.TryParse(Path.Substring(S, I - S), out var Idx)) return null;
                    I++;

                    if ((Cur.ValueKind != JsonValueKind.Array) || (Idx < 0) || (Idx >= Cur.GetArrayLength()))
                        return null;

                    Cur = Cur[Idx];
                }
                else
                    I++;
            }

            return (Cur.ValueKind == JsonValueKind.String) ? Cur.GetString() : Cur.ToString();
        }
    }
}
