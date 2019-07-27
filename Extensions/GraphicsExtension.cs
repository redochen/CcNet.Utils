using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Graphics扩展方法
    /// </summary>
    public static class GraphicsExtension
    {
        /// <summary>
        /// 画方角矩形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="fillBrush">填充画刷</param>
        /// <param name="borderPen">边框画笔</param>
        /// <param name="borderSize">边框大小</param>
        public static void DrawCornerRect(this Graphics graphics, Rectangle rect, Brush fillBrush, Pen borderPen, int borderSize)
        {
            if (null == graphics)
            {
                return;
            }

            if (borderPen != null)
            {
                graphics.DrawRectangle(borderPen, rect);
            }

            if (fillBrush != null)
            {
                graphics.FillRectangle(fillBrush, rect);
            }
        }

        /// <summary>
        /// 画圆角矩形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="fillBrush">填充画刷</param>
        /// <param name="borderPen">边框画笔</param>
        /// <param name="borderSize">边框大小</param>
        /// <param name="radius">圆角半径</param>
        public static void DrawRoundRect(this Graphics graphics, Rectangle rect, Brush fillBrush, Pen borderPen, int borderSize, int radius)
        {
            if (null == graphics || radius <= 0)
            {
                return;
            }

            //抗锯齿
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (borderPen != null)
            {
                graphics.DrawPath(borderPen, rect.GetRoundedRectPath(radius));
            }

            if (fillBrush != null)
            {
                graphics.FillPath(fillBrush, rect.GetRoundedRectPath(radius));
            }
        }

        /// <summary>
        /// 画矩形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="fillBrush">填充画刷</param>
        /// <param name="borderPen">边框画笔</param>
        /// <param name="borderSize">边框大小</param>
        /// <param name="radius">圆角半径</param>
        public static void DrawRect(this Graphics graphics, Rectangle rect, Brush fillBrush, Pen borderPen, int borderSize, int radius)
        {
            if (null == graphics)
            {
                return;
            }

            if (radius > 0)
            {
                graphics.DrawRoundRect(rect, fillBrush, borderPen, borderSize, radius);
            }
            else
            {
                graphics.DrawCornerRect(rect, fillBrush, borderPen, borderSize);
            }
        }

        /// <summary>
        /// 画文字
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="text">文字内容</param>
        /// <param name="font">字体</param>
        /// <param name="color">文字颜色</param>
        /// <param name="flags"></param>
        public static void DrawText(this Graphics graphics, Rectangle rect, string text, Font font, Color color, TextFormatFlags flags = TextFormatFlags.Default)
        {
            if (null == graphics || null == font || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            //抗锯齿
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var size = TextRenderer.MeasureText(text, font);

            TextRenderer.DrawText(graphics, text, font, rect, color, flags);
        }

        #region 画按钮

        /// <summary>
        /// 画按钮的方角矩形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="fillBrush">填充画刷</param>
        /// <param name="borderPen">边框画笔</param>
        /// <param name="borderSize">边框大小</param>
        /// <param name="borderOpen">边框开放的方向</param>
        /// <param name="openPen">开放画笔</param>
        public static void DrawButtonCornerRect(this Graphics graphics, Rectangle rect, Brush fillBrush,
            Pen borderPen, int borderSize, CcCtrlPosition borderOpen = CcCtrlPosition.None, Pen openPen = null)
        {
            if (null == graphics)
            {
                return;
            }

            //不抗锯齿
            graphics.SmoothingMode = SmoothingMode.None;

            var drawBorder = (borderSize > 0 && borderPen != null);
            var offset = (drawBorder && 1 == borderSize) ? Win32.SIZE_SYSTEM : 0;

            var drawRect = new Rectangle
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width - offset,
                Height = rect.Height - offset
            };

            if (fillBrush != null)
            {
                graphics.FillRectangle(fillBrush, drawRect);
            }

            if (drawBorder)
            {
                graphics.DrawRectangle(borderPen, drawRect);
                graphics.DrawOpenLine(drawRect, fillBrush, borderSize, 0, borderOpen, openPen);
            }
        }

        /// <summary>
        /// 画按钮的圆角矩形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="fillBrush">填充画刷</param>
        /// <param name="borderPen">边框画笔</param>
        /// <param name="borderSize">边框大小</param>
        /// <param name="radius">圆角半径</param>
        /// <param name="radiusCorners">圆角数量</param>
        /// <param name="borderOpen">边框开放的方向</param>
        /// <param name="openPen">开放画笔</param>
        public static void DrawButtonRoundRect(this Graphics graphics, Rectangle rect, Brush fillBrush
            , Pen borderPen, int borderSize, int radius, CcCtrlCorner radiusCorners = CcCtrlCorner.All
            , CcCtrlPosition borderOpen = CcCtrlPosition.None, Pen openPen = null)
        {
            if (null == graphics || radius <= 0)
            {
                return;
            }

            //抗锯齿
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var drawBorder = (borderSize > 0 && borderPen != null);
            var delta = Win32.SIZE_SYSTEM; //抗锯齿模式下，系统有1个像素的渐变

            var drawRect = new Rectangle
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width - delta,
                Height = rect.Height - delta
            };

            if (fillBrush != null)
            {
                if (drawBorder)
                {
                    var offset = (int)Math.Floor(borderSize / 2.0F);

                    var fillRect = new Rectangle
                    {
                        X = rect.X + offset,
                        Y = rect.Y + offset,
                        Width = rect.Width - borderSize,
                        Height = rect.Height - borderSize
                    };

                    graphics.FillPath(fillBrush, fillRect.GetRoundedRectPath(radius, radiusCorners));
                }
                else
                {
                    graphics.FillPath(fillBrush, drawRect.GetRoundedRectPath(radius, radiusCorners));
                }
            }

            if (drawBorder)
            {
                graphics.DrawPath(borderPen, drawRect.GetRoundedRectPath(radius, radiusCorners));
                graphics.DrawOpenLine(rect, fillBrush, borderSize, radius, borderOpen, openPen);
            }
        }

        /// <summary>
        /// 画按钮的矩形
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="fillBrush">填充画刷</param>
        /// <param name="borderPen">边框画笔</param>
        /// <param name="borderSize">边框大小</param>
        /// <param name="radius">圆角半径</param>
        /// <param name="borderOpen">边框开放的方向</param>
        /// <param name="borderOpen">边框开放的方向</param>
        /// <param name="openPen">开放画笔</param>
        public static void DrawButtonRect(this Graphics graphics, Rectangle rect, Brush fillBrush
            , Pen borderPen, int borderSize, int radius, CcCtrlCorner radiusCorners = CcCtrlCorner.All
            , CcCtrlPosition borderOpen = CcCtrlPosition.None, Pen openPen = null)
        {
            if (null == graphics)
            {
                return;
            }

            if (radius > 0)
            {
                graphics.DrawButtonRoundRect(rect, fillBrush, borderPen, borderSize, radius, radiusCorners, borderOpen, openPen);
            }
            else
            {
                graphics.DrawButtonCornerRect(rect, fillBrush, borderPen, borderSize, borderOpen, openPen);
            }
        }

        /// <summary>
        /// 画按钮的文字
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="text">文字内容</param>
        /// <param name="font">字体</param>
        /// <param name="color">文字颜色</param>
        public static void DrawButtonText(this Graphics graphics, Rectangle rect, string text, Font font, Color color)
        {
            if (null == graphics || null == font || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            //抗锯齿
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var size = TextRenderer.MeasureText(text, font);

            var textX = rect.X + (rect.Width - size.Width) / 2;
            var textY = rect.Y + (rect.Height - size.Height) / 2;
            var textRect = new Rectangle(textX, textY, size.Width, size.Height);

            TextRenderer.DrawText(graphics, text, font, textRect, color);
        }

        /// <summary>
        /// 画按钮
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">矩形区域</param>
        /// <param name="fillBrush">填充画刷</param>
        /// <param name="borderPen">边框画笔</param>
        /// <param name="borderSize">边框大小</param>
        /// <param name="radius">圆角半径</param>
        /// <param name="radiusCorners">圆角数量</param>
        /// <param name="font">字体</param>
        /// <param name="textColor">文字颜色</param>
        /// <param name="text">文字内容</param>
        /// <param name="borderOpen">边框开放的方向</param>
        /// <param name="openPen">开放画笔</param>
        public static void DrawButton(this Graphics graphics, Rectangle rect, Brush fillBrush
            , Pen borderPen, int borderSize, int radius, CcCtrlCorner radiusCorners
            , Font font, Color textColor, string text, CcCtrlPosition borderOpen = CcCtrlPosition.None, Pen openPen = null)
        {
            //画矩形
            graphics.DrawButtonRect(rect, fillBrush, borderPen, borderSize, radius, radiusCorners, borderOpen, openPen);

            //画文字
            graphics.DrawButtonText(rect, text, font, textColor);
        }

        /// <summary>
        /// 画开放边框的直线
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect"></param>
        /// <param name="fillBrush"></param>
        /// <param name="borderSize"></param>
        /// <param name="radius">圆角半径</param>
        /// <param name="borderOpen"></param>
        /// <param name="openPen"></param>
        private static void DrawOpenLine(this Graphics graphics, Rectangle rect, Brush fillBrush, int borderSize, int radius, CcCtrlPosition borderOpen, Pen openPen = null)
        {
            var points = rect.GetOpenLine(borderOpen, borderSize, radius);
            if (null == points)
            {
                return;
            }

            //画圆角时留一个像素给系统渐变
            var offset = radius > 0 ? Win32.SIZE_SYSTEM : 0;

            var newPen = (null == openPen);
            if (newPen)
            {
                if (fillBrush != null)
                {
                    openPen = new Pen(fillBrush, borderSize + offset);
                }
                else
                {
                    openPen = new Pen(Color.White, borderSize + offset);
                }
            }

            if (radius > 0)
            {
                //graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
            else
            {
                graphics.SmoothingMode = SmoothingMode.None;
            }

            graphics.DrawLine(openPen, points.Item1, points.Item2);

            if (newPen)
            {
                openPen.Dispose();
            }
        }

        #endregion
    }
}