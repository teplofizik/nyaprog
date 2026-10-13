using System.Collections.Generic;
using System.Linq;

using Programmer.Options;

namespace Programmer.Modules
{
    /// <summary>
    /// Необязательная база для инструментов: хранит команды, опции, архитектуры и теги.
    /// Наследование не обязательно — достаточно реализовать <see cref="ITool"/>.
    /// </summary>
    public abstract class ToolBase : ITool
    {
        private readonly List<string> mArch = new List<string>();
        private readonly List<string> mTags = new List<string>();
        private readonly List<Option> mOptions = new List<Option>();
        private readonly List<ICommand> mCommands = new List<ICommand>();

        protected ToolBase(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public string Id { get; }

        public string Name { get; }

        public virtual ToolKind Kind => ToolKind.Utility;

        public string[] SupportedArch => mArch.ToArray();

        public string[] Tags => mTags.ToArray();

        public virtual Option[] GetOptions() => mOptions.ToArray();

        public virtual ICommand[] GetCommands() => mCommands.ToArray();

        /// <summary>
        /// Пустой список архитектур трактуется как «без ограничения».
        /// </summary>
        public bool SupportsArch(string[] Arch) => (mArch.Count == 0) || Arch.Any(A => mArch.Contains(A));

        protected void AddArch(string A) => mArch.Add(A);

        protected void AddTag(string T) => mTags.Add(T);

        protected void AddOption(Option O) => mOptions.Add(O);

        protected void AddCommand(ICommand C) => mCommands.Add(C);

        public override string ToString() => Name;
    }
}
