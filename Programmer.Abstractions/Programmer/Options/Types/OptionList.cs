using Extension.Argument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Options.Types
{
    public class OptionList : Option
    {
        private List<OptionListItem> mStorage = new List<OptionListItem>();

        public OptionList(string Name) : base(Name)
        {

        }

        /// <summary>
        /// Список вариантов
        /// </summary>
        public OptionListItem[] Items
        {
            get { return mStorage.ToArray(); }
        }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        /// <returns></returns>
        public override ArgumentList GetDefault()
        {
            var Default = Items[0];
            foreach (var I in Items)
            {
                if (I.Default)
                {
                    Default = I;
                    break;
                }
            }

            var A = new ArgumentList();
            foreach(var K in Default.getNames())
                A.Set(K, Default.GetValue(K));
            return A;
        }

        /// <summary>
        /// Добавить элемент в список
        /// </summary>
        /// <param name="I"></param>
        public void AddItem(OptionListItem I)
        {
            mStorage.Add(I);
        }
    }
}
