using Programmer.Project;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Отображаемый шаг плана прошивки.
    /// </summary>
    public sealed class StepItem
    {
        public Step Step { get; }

        public StepItem(Step Step) { this.Step = Step; }

        public string Title => Step.Title;

        /// <summary>Шаг выполняется сейчас.</summary>
        public bool Running { get; set; }

        /// <summary>Сообщение результата.</summary>
        public string Result { get; set; }

        /// <summary>Успех/неудача (null — ещё не выполнялся).</summary>
        public bool? Ok { get; set; }
    }
}
