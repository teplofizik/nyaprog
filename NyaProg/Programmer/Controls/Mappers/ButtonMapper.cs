using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Programmer.Controls.Mappers
{
    public class ButtonMapper : Mapper
    {
        /// <summary>
        /// Ширина кнопки
        /// </summary>
        const int ButtonWidth = 200;

        /// <summary>
        /// Высота кнопки
        /// </summary>
        const int ButtonHeight = 22;

        /// <summary>
        /// Расстояние между кнопками
        /// </summary>
        const int ButtonSpacing = 9;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="W">Ширина элемента</param>
        /// <param name="H">Высота элемента</param>
        /// <param name="L">Начальное смещение слева</param>
        /// <param name="T">Начальное смещение сверху</param>
        /// <param name="dX">Расстояние между левыми частями элементов по горизонтали</param>
        /// <param name="dY">Расстояние между верхними частями элементов по вертикали</param>
        public ButtonMapper(int L, int T) 
            : base(ButtonWidth, ButtonHeight, L, T, 0, ButtonHeight + ButtonSpacing)
        {

        }
        
        /// <summary>
        /// Создание нового элемента (определяется потомками класса)
        /// </summary>
        /// <returns></returns>
        protected override Control CreateElement() => new Button();
    }
}
