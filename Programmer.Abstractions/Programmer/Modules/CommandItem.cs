using Extension.Argument;

namespace Programmer.Modules
{
    /// <summary>
    /// Элемент списка, возвращаемого командой-поставщиком (для динамических опций):
    /// отображаемая подпись и набор значений-аргументов.
    /// </summary>
    public sealed class CommandItem
    {
        /// <summary>Отображаемая подпись.</summary>
        public string Label;

        /// <summary>Значения-аргументы, устанавливаемые при выборе пункта.</summary>
        public ArgumentList Values;

        public CommandItem(string Label, ArgumentList Values = null)
        {
            this.Label = Label ?? "";
            this.Values = Values ?? new ArgumentList();
        }

        public override string ToString() => Label;
    }
}
