using System.Collections.Generic;

namespace Programmer.Project
{
    /// <summary>
    /// Шаг ветвления: <c>&lt;if cond="..."&gt; ... &lt;else&gt; ... &lt;/else&gt; &lt;/if&gt;</c>.
    /// Ветка выбирается при построении плана этапов (по текущим опциям/окружению).
    /// </summary>
    public class IfStep : Step
    {
        /// <summary>Условие ветвления (выражение).</summary>
        public string Condition;

        /// <summary>Шаги ветки «тогда».</summary>
        public List<Step> Then = new List<Step>();

        /// <summary>Шаги ветки «иначе».</summary>
        public List<Step> Else = new List<Step>();

        public override string Title => Comment ?? "if";
    }
}
