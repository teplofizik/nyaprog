﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Programmer.Controls.Mappers
{
    public class OptionMapper : Mapper
    {
        /// <summary>
        /// Высота поля
        /// </summary>
        const int OptionHeight = 22;

        /// <summary>
        /// Расстояние между полями ввода
        /// </summary>
        const int OptionSpacing = 9;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="W">Ширина элемента</param>
        /// <param name="H">Высота элемента</param>
        /// <param name="L">Начальное смещение слева</param>
        /// <param name="T">Начальное смещение сверху</param>
        /// <param name="dX">Расстояние между левыми частями элементов по горизонтали</param>
        /// <param name="dY">Расстояние между верхними частями элементов по вертикали</param>
        public OptionMapper(int L, int T, int W) 
            : base(W, OptionHeight, L, T, 0, OptionHeight + OptionSpacing)
        {

        }
        
        /// <summary>
        /// Создание нового элемента (определяется потомками класса)
        /// </summary>
        /// <returns></returns>
        protected override Control CreateElement() => new OptionViewer();
    }
}
