using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CcNet.Utils.Controls
{
    public abstract class CcTransparent : Panel
    {
        /// <summary>
        /// Drawing surface where graphics should be drawn.
        /// Use this member in the OnDraw method.
        /// </summary>
        protected Graphics graphics;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 

                return cp;
            }
        }

        public CcTransparent()
        {
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Don't paint background
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Update the private member so we can use it in the OnDraw method
            graphics = e.Graphics;

            // Set the best settings possible (quality-wise)
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            // Calls the OnDraw subclass method
            OnDraw();
        }

        /// <summary>
        /// Override this method in subclasses for drawing purposes.
        /// </summary>
        abstract protected void OnDraw();

        #region DrawText helper functions

        protected void DrawText(string text, Point position)
        {
            DrawText(graphics, text, "Microsoft Sans Serif", 8.25f, FontStyle.Regular, Brushes.Black, position);
        }

        protected void DrawText(string text, Brush color, Point position)
        {
            DrawText(graphics, text, "Microsoft Sans Serif", 8.25f, FontStyle.Regular, color, position);
        }

        protected void DrawText(string text, FontStyle style, Point position)
        {
            DrawText(graphics, text, "Microsoft Sans Serif", 8.25f, style, Brushes.Black, position);
        }

        protected void DrawText(string text, FontStyle style, Brush color, Point position)
        {
            DrawText(graphics, text, "Microsoft Sans Serif", 8.25f, style, color, position);
        }

        protected void DrawText(string text, float fontSize, FontStyle style, Brush color, Point position)
        {
            DrawText(graphics, text, "Microsoft Sans Serif", fontSize
                , style, color, position);
        }

        protected void DrawText(string text, string fontFamily, float fontSize, FontStyle style, Brush color, Point position)
        {
            DrawText(graphics, text, fontFamily, fontSize, style, color, position);
        }

        public static void DrawText(Graphics graphics, string text, string fontFamily, float fontSize, FontStyle style, Brush color, Point position)
        {
            Font font = new Font(fontFamily, fontSize, style);

            SizeF textSizeF = graphics.MeasureString(text, font);
            int width = (int)Math.Ceiling(textSizeF.Width);
            int height = (int)Math.Ceiling(textSizeF.Height);
            Size textSize = new Size(width, height);
            Rectangle rectangle = new Rectangle(position, textSize);

            graphics.DrawString(text, font, color, rectangle);
        }

        #endregion
    }
}