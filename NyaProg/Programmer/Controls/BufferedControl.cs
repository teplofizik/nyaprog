using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Programmer.Controls
{
    public class BufferedControl : Control
    {
        /// <summary>
        /// Буферизованное изображение
        /// </summary>
        protected Image mBuffer;

        public BufferedControl()
        {
            //Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            SetStyle(ControlStyles.EnableNotifyMessage, true);

            mBuffer = new Bitmap(10, 10);
        }

        public new void Invalidate()
        {
            base.Invalidate();
            if((Width > 0) && (Height > 0))
            using (Graphics G = Graphics.FromImage(mBuffer))
            {
                OnBufferPaint(new PaintEventArgs(G, ClientRectangle));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImageUnscaledAndClipped(mBuffer, ClientRectangle);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int W = Math.Max(Width, 1);
            int H = Math.Max(Height, 1);
            mBuffer = new Bitmap(W, H);

            Invalidate();
        }

        /// <summary>
        /// Рисование в буфер
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBufferPaint(PaintEventArgs e)
        {

        }
    }
}
