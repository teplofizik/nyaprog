namespace Programmer.Project
{
    /// <summary>
    /// Шаг присвоения переменной окружения: <c>&lt;set name="url" value="http://{host}/fw" /&gt;</c>.
    /// Значение — шаблон с <c>{переменными}</c>, разрешается на момент выполнения.
    /// </summary>
    public class SetStep : Step
    {
        /// <summary>Имя переменной.</summary>
        public string Name;

        /// <summary>Значение (шаблон).</summary>
        public string Value;

        public override string Title => Comment ?? $"set {Name} = {Value}";
    }
}
