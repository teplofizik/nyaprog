using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Programmer.Controls.Mappers
{
    public class StepMapper : Mapper
    {
        /// <summary>
        /// Ширина кнопки
        /// </summary>
        const int StepWidth = 180;

        /// <summary>
        /// Высота кнопки
        /// </summary>
        const int StepHeight = 17;

        /// <summary>
        /// Расстояние между чекбоксами
        /// </summary>
        const int StepSpacing = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="W">Ширина элемента</param>
        /// <param name="H">Высота элемента</param>
        /// <param name="L">Начальное смещение слева</param>
        /// <param name="T">Начальное смещение сверху</param>
        /// <param name="dX">Расстояние между левыми частями элементов по горизонтали</param>
        /// <param name="dY">Расстояние между верхними частями элементов по вертикали</param>
        public StepMapper(int L, int T) 
            : base(StepWidth, StepHeight, L, T, 0, StepHeight + StepSpacing)
        {

        }
        
        /// <summary>
        /// Создание нового элемента (определяется потомками класса)
        /// </summary>
        /// <returns></returns>
        protected override Control CreateElement() => new StepControl();
    }
}
