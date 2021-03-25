using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Programmer.Controls
{
    /// <summary>
    /// Класс, отвечающий за размеры и позиционирование последовательно создаваемых элементов
    /// </summary>
    public abstract class Mapper
    {
        /// <summary>
        /// Номер элемента
        /// </summary>
        private int Index = 0;

        /// <summary>
        /// Ширина элемента
        /// </summary>
        protected int Width;

        /// <summary>
        /// Высота элемента
        /// </summary>
        protected int Height;

        /// <summary>
        /// Расстояние между левыми частями элементов по горизонтали
        /// </summary>
        protected int dX;

        /// <summary>
        /// Расстояние между верхними частями элементов по вертикали
        /// </summary>
        protected int dY;

        /// <summary>
        /// Начальное смещение слева
        /// </summary>
        protected int Left;

        /// <summary>
        /// Начальное смещение сверху
        /// </summary>
        protected int Top;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="W">Ширина элемента</param>
        /// <param name="H">Высота элемента</param>
        /// <param name="L">Начальное смещение слева</param>
        /// <param name="T">Начальное смещение сверху</param>
        /// <param name="dX">Расстояние между левыми частями элементов по горизонтали</param>
        /// <param name="dY">Расстояние между верхними частями элементов по вертикали</param>
        public Mapper(int W, int H, int L, int T, int dX, int dY)
        {
            Width = W;
            Height = H;
            Left = L;
            Top = T;
            this.dX = dX;
            this.dY = dY;
        }

        /// <summary>
        /// Создание нового элемента (определяется потомками класса)
        /// </summary>
        /// <returns></returns>
        protected abstract Control CreateElement();

        /// <summary>
        /// Сброс счётчика
        /// </summary>
        public void Reset()
        {
            Index = 0;
        }

        /// <summary>
        /// Создать очередной контрол
        /// </summary>
        /// <returns></returns>
        public Control CreateNextControl(string Text, object Tag)
        {
            var C = CreateElement();

            C.Tag = Tag;
            C.Text = Text;
            C.Width = Width;
            C.Height = Height;
            C.Top = Top + dY * Index;
            C.Left = Left + dX * Index;

            Index++;
            return C;
        }
    }
}
