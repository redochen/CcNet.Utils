using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义按钮类
    /// </summary>
    [ToolboxItem(true)]
    public class CcButton : Button, IStateAppearance, IBorderSupport
    {
        #region 属性
        #region 被隐藏的属性
        [Browsable(false)]
        public new Font Font { get; set; }

        [Browsable(false)]
        public new Color ForeColor { get; set; }

        [Browsable(false)]
        public new Color BackColor { get; set; }
        #endregion

        /// <summary>
        /// 边框
        /// </summary>
        private CcBorder border = CcBorder.DefaultBorder;
        [Category("CC外观"), Description("获取或设置边框属性"), RefreshProperties(RefreshProperties.All)]
        public CcBorder Border
        {
            get => border;
            set
            {
                border = value;
                Refresh();
            }
        }

        /// <summary>
        /// 外观
        /// </summary>
        private CcAppearances appearances = CcAppearances.DefaultAppearances;
        [Category("CC外观"), Description("获取或设置外观属性"), RefreshProperties(RefreshProperties.All)]
        public CcAppearances Appearances
        {
            get => appearances;
            set
            {
                appearances = value;
                Refresh();
            }
        }

        /// <summary>
        /// 图片宽度
        /// </summary>
        private int imageWidth = 18;
        [Category("CC外观"), Description("获取或设置图片宽度"), RefreshProperties(RefreshProperties.All)]
        public int ImageWidth
        {
            get => imageWidth;
            set
            {
                imageWidth = value;
                Refresh();
            }
        }

        /// <summary>
        /// 图片高度
        /// </summary>
        private int imageHeight = 18;
        [Category("CC外观"), Description("获取或设置图片高度"), RefreshProperties(RefreshProperties.All)]
        public int ImageHeight
        {
            get => imageHeight;
            set
            {
                imageHeight = value;
                Refresh();
            }
        }

        /// <summary>
        /// 图片与文字的间距
        /// </summary>
        private int imageTextPadding = 0;
        [Category("CC外观"), Description("获取或设置图片与文字的间距"), RefreshProperties(RefreshProperties.All)]
        public int ImageTextPadding
        {
            get => imageTextPadding;
            set
            {
                imageTextPadding = value;
                Refresh();
            }
        }
        #endregion

        /// <summary>
        /// 控件助手实例
        /// </summary>
        private readonly CtrlAssist<CcButton> Assist = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CcButton() : base()
        {
            SetStyle(CtrlAssist<CcButton>.GetStyles(userPaint: true, supportsTransparent: true), true);
            Assist = new CtrlAssist<CcButton>(this, setTransparent: true);
        }

        /// <summary>
        /// 判断指定点是否在按钮里
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool HitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        /// <summary>
        /// 描绘
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="rect">矩形</param>
        public void Draw(Graphics graphic, Rectangle rect)
        {
            Location = rect.Location;
            Size = rect.Size;

            Draw(graphic);
        }

        /// <summary>
        /// 描绘
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public void Draw(Graphics graphics, int x, int y)
        {
            Location = new Point(x, y);

            Draw(graphics);
        }

        /// <summary>
        /// 描绘
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void Draw(Graphics graphics, int x, int y, int width, int height)
        {
            Location = new Point(x, y);
            Size = new Size(width, height);

            Draw(graphics);
        }

        #region 事件
        /// <summary>
        /// 描绘事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            base.OnPaintBackground(e);

            Draw(e.Graphics);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion

        /// <summary>
        /// 绘图
        /// </summary>
        /// <param name="graphics"></param>
        private void Draw(Graphics graphics)
        {
            Pen borderPen = null;
            Brush fillBrush = null;

            try
            {
                if (Bounds.Width <= 0 && Bounds.Height <= 0)
                {
                    return;
                }

                CalculateRect(out Rectangle imageRect, out Rectangle textRect);

                borderPen = Assist.BorderPen;
                fillBrush = Assist.FillBrush;

                //画矩形
                graphics.DrawButtonRect(ClientRectangle, fillBrush, borderPen, Border.Size,
                    Border.Radius, Border.RadiusCorners, Border.OpenDirection, null);

                if (Image != null)
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    graphics.DrawImage(Image, imageRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
                }

                var flags = SkinUtil.GetTextFormatFlags(TextAlign, RightToLeft.Yes == RightToLeft, true);

                TextRenderer.DrawText(graphics, Text, Assist.TextFont, textRect, Assist.TextColor, flags);
            }
            catch (Exception ex) { }
            finally
            {
                borderPen?.Dispose();
                fillBrush?.Dispose();
            }
        }

        private void CalculateRect(out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;

            var textSize = TextRenderer.MeasureText(Text, Assist.TextFont);

            if (null == Image)
            {
                textRect = new Rectangle((Width - textSize.Width) / 2, (Height - textSize.Height) / 2,
                    textSize.Width, textSize.Height);
                return;
            }

            switch (TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    {
                        imageRect = new Rectangle(
                            (Width - ImageWidth) / 2,
                            (Height - ImageHeight) / 2,
                           ImageWidth, ImageHeight);

                        textRect = new Rectangle(
                            (Width - textSize.Width) / 2,
                            (Height - textSize.Height) / 2,
                            textSize.Width, textSize.Height);
                    }
                    break;
                case TextImageRelation.ImageAboveText:
                    {
                        var totalHeight = ImageHeight + ImageTextPadding + textSize.Height;

                        imageRect = new Rectangle(
                            (Width - ImageWidth) / 2,
                            (Height - totalHeight) / 2,
                            ImageWidth, ImageHeight);

                        textRect = new Rectangle(
                            (Width - textSize.Width) / 2,
                            imageRect.Bottom + ImageTextPadding,
                            textSize.Width, textSize.Height);
                    }
                    break;
                case TextImageRelation.ImageBeforeText:
                    {
                        var totalWidth = ImageWidth + ImageTextPadding + textSize.Width;

                        imageRect = new Rectangle(
                            (Width - totalWidth) / 2,
                            (Height - ImageHeight) / 2,
                            ImageWidth, ImageHeight);

                        textRect = new Rectangle(
                            imageRect.Right + ImageTextPadding,
                            (Height - textSize.Height) / 2,
                            textSize.Width, textSize.Height);
                    }
                    break;
                case TextImageRelation.TextAboveImage:
                    {
                        var totalHeight = textSize.Height + ImageTextPadding + ImageHeight;

                        textRect = new Rectangle(
                            (Width - textSize.Width) / 2,
                            (Height - totalHeight) / 2,
                            textSize.Width, textSize.Height);

                        imageRect = new Rectangle(
                            (Width - ImageWidth) / 2,
                            textRect.Bottom + ImageTextPadding,
                            ImageWidth, ImageHeight);
                    }
                    break;
                case TextImageRelation.TextBeforeImage:
                    {
                        var totalWidth = textSize.Width + ImageTextPadding + ImageWidth;

                        textRect = new Rectangle(
                            (Width - totalWidth) / 2,
                            (Height - textSize.Height) / 2,
                            textSize.Width, textSize.Height);

                        imageRect = new Rectangle(
                            textRect.Right + ImageTextPadding,
                            (Height - ImageHeight) / 2,
                            ImageWidth, ImageHeight);
                    }
                    break;
            }

            if (RightToLeft.Yes == RightToLeft)
            {
                imageRect.X = Width - imageRect.Right;
                textRect.X = Width - textRect.Right;
            }
        }
    }
}