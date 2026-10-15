using Programmer.Modules;

namespace Programmer.Modules.Internal
{
    /// <summary>
    /// Диалоги с пользователем (message/question) через IUserInterface.
    /// </summary>
    public sealed class MessageTool : CodedToolBase
    {
        public MessageTool() : base("core.messages", "Messages")
        {
            AddCommand(new ShowMessageCommand());
            AddCommand(new QuestionCommand());
        }
    }

    sealed class ShowMessageCommand : CommandBase
    {
        public ShowMessageCommand() : base("message", "Сообщение") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            Ctx.Host.UI.Message(Ctx.Args.Get("value") ?? "", "Сообщение");
            return CommandResult.Ok();
        }
    }

    sealed class QuestionCommand : CommandBase
    {
        public QuestionCommand() : base("question", "Вопрос") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Result = Ctx.Host.UI.Question(Ctx.Args.Get("value") ?? "", "Вопрос");
            return Result ? CommandResult.Ok() : CommandResult.Error("Cancelled");
        }
    }
}
