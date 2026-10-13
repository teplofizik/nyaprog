using System.Collections.Generic;

using Programmer.Modules;

namespace Programmer.Modules.Internal
{
    /// <summary>
    /// Модуль встроенных (кодовых) инструментов ядра.
    /// </summary>
    public sealed class InternalModule : ModuleBase
    {
        public override string Id => "core.internal";

        public override string Name => "Internal";

        public override IEnumerable<ITool> GetTools() => new ITool[]
        {
            new FileOpsTool(),
            new MessageTool(),
            new ExternalTool()
        };
    }
}
