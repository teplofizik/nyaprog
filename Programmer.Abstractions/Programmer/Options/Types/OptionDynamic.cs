using System.Collections.Generic;

using Extension.Argument;
using Programmer.Modules;

namespace Programmer.Options.Types
{
    /// <summary>
    /// Динамический список: значения подгружаются в рантайме командой-поставщиком
    /// (например, сканирование сети). Выбранный пункт задаёт набор аргументов.
    /// </summary>
    public class OptionDynamic : Option
    {
        /// <summary>
        /// Название параметра (плейсхолдер {имя}) — для команды-поставщика.
        /// </summary>
        public string ParamName = "";

        /// <summary>
        /// Идентификатор команды-поставщика списка.
        /// </summary>
        public string Source = "";

        /// <summary>
        /// Подпись пункта, выбираемого по умолчанию.
        /// </summary>
        public string DefaultLabel = null;

        /// <summary>
        /// Загруженные пункты (заполняется хостом).
        /// </summary>
        public IReadOnlyList<CommandItem> Items = new CommandItem[0];

        public override ArgumentList GetDefault()
        {
            var A = new ArgumentList();
            var Item = FindDefault();

            if (Item != null)
                A.Append(Item.Values);
            else
                A.Set(ParamName, "");

            return A;
        }

        private CommandItem FindDefault()
        {
            if ((Items == null) || (Items.Count == 0)) return null;

            if (DefaultLabel != null)
            {
                foreach (var I in Items)
                    if (I.Label == DefaultLabel) return I;
            }

            return Items[0];
        }

        public OptionDynamic(string Name) : base(Name)
        {
        }
    }
}
