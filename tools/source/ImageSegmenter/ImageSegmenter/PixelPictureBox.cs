using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specialized PictureBox that displays pixels as blocks when zooming in rather than interpolating levels in between.
    /// </summary>
    public class PixelPictureBox : PictureBox
    {
        /// <summary>
        /// Override OnPaint method to change the Graphics modes to display pixel blocks.
        /// </summary>
        /// <param name="e">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            base.OnPaint(e);
        }
        /// <summary>
        /// Override OnPaintBackground method to change the Graphics modes to display pixel blocks.
        /// </summary>
        /// <param name="e">Paint Event Arguments</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            base.OnPaintBackground(e);
        }

    }
}
