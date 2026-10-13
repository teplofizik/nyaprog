using Extension.Argument;

namespace Programmer.Options.Types
{
    /// <summary>
    /// Флажок (чекбокс). Когда включён — параметр равен <see cref="Value"/>,
    /// иначе — пустая строка (что удобно для условий <c>if cond="имя"</c>).
    /// </summary>
    public class OptionBool : Option
    {
        /// <summary>
        /// Название параметра (плейсхолдер {имя}).
        /// </summary>
        public string ParamName = "";

        /// <summary>
        /// Включён ли по умолчанию.
        /// </summary>
        public bool Default = false;

        /// <summary>
        /// Значение параметра во включённом состоянии.
        /// </summary>
        public string Value = "1";

        public override ArgumentList GetDefault()
        {
            var A = new ArgumentList();
            A.Set(ParamName, Default ? Value : "");
            return A;
        }

        public OptionBool(string Name) : base(Name)
        {
        }
    }
}
