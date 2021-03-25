using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Programmer.Tool;

namespace Programmer.Controls
{
    public class StepControl : BufferedControl
    {
        /// <summary>
        /// Результат выполнения операции
        /// </summary>
        public ExecResult Result;

        /// <summary>
        /// В процессе выполнения
        /// </summary>
        public bool Process = false;

        /// <summary>
        /// Кисть для кружочка
        /// </summary>
        /// <returns></returns>
        protected Brush IndBrush
        {
            get
            {
                if (Process)
                    return Brushes.White;
                if (Result == null)
                    return Brushes.Gray;
                switch(Result.Result)
                {
                    case ExecResultType.Ok: return Brushes.Green;
                    case ExecResultType.Warning: return Brushes.Yellow;
                    case ExecResultType.Error: return Brushes.Red;
                    default: return Brushes.Blue;
                }

            }
        }

        /// <summary>
        /// Кисть для текста
        /// </summary>
        /// <returns></returns>
        protected Brush TextBrush
        {
            get
            {
                if (Result == null)
                    return Brushes.Black;
                switch (Result.Result)
                {
                    case ExecResultType.Ok: return Brushes.DarkGreen;
                    case ExecResultType.Warning: return Brushes.Brown;
                    case ExecResultType.Error: return Brushes.DarkRed;
                    default: return Brushes.Black;
                }

            }
        }

        /// <summary>
        /// Рисование в буфер
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBufferPaint(PaintEventArgs e)
        {
            // Размер кружочка индикации
            int IndSize = Height * 2 / 3;
            var Ind = new Rectangle(5, (Height - IndSize) / 2, IndSize, IndSize);

            var G = e.Graphics;
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            G.FillRectangle(SystemBrushes.Control, e.ClipRectangle);
            G.FillEllipse(IndBrush, Ind);
            G.DrawEllipse(Pens.Black, Ind);
            {
                var Sz = G.MeasureString(Text, Font);
                G.DrawString(Text, Font, TextBrush, 5 + IndSize + 5, (Height - Sz.Height) / 2);
            }
        }
    }
}
