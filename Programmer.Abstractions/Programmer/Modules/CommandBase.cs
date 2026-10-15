namespace Programmer.Modules
{
    /// <summary>
    /// Необязательная база для команд: заполняет Id и Descriptor.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public string Id { get; }

        public CommandDescriptor Descriptor { get; }

        protected CommandBase(string Id, string Title = null)
        {
            this.Id = Id;
            Descriptor = new CommandDescriptor(Id) { Title = Title ?? Id };
        }

        public abstract CommandResult Execute(CommandContext Ctx);

        public override string ToString() => Id;
    }
}
