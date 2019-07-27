using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义进度条类
    /// </summary>
    [ToolboxItem(true)]
    public class CcProgressBar : ProgressBar, IStateAppearance, IBorderSupport
    {
        #region 属性
        #region 被隐藏的属性
        [Browsable(false)]
        public new Font Font { get; set; }

        [Browsable(false)]
        public new Color ForeColor { get; set; }
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
        /// 文字对齐方式
        /// </summary>
        private HorizontalAlignment textAlign = HorizontalAlignment.Center;
        [Category("CC外观"), Description("获取或设置文字对齐方式"), RefreshProperties(RefreshProperties.All)]
        public HorizontalAlignment TextAlign
        {
            get => textAlign;
            set
            {
                textAlign = value;
                Refresh();
            }
        }

        /// <summary>
        /// 文本内容
        /// </summary>
        [Category("CC外观"), Description("获取或设置文本内容"), RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Refresh();
            }
        }
        #endregion

        /// <summary>
        /// 控件助手实例
        /// </summary>
        private readonly CtrlAssist<CcProgressBar> Assist = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CcProgressBar() : base()
        {
            SetStyle(CtrlAssist<CcProgressBar>.GetStyles(userPaint: true, supportsTransparent: true), true);
            Assist = new CtrlAssist<CcProgressBar>(this, setTransparent: true);
        }

        /// <summary>
        /// 描绘事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            //base.OnPaintBackground(e);

            Draw(e.Graphics, e.ClipRectangle);
        }

        /// <summary>
        /// 绘图
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectangle"></param>
        private void Draw(Graphics graphics, Rectangle rectangle)
        {
            Pen borderPen = null;
            Brush fillBrush = null;
            Brush scaleBrush = null;

            try
            {
                var rect = rectangle;

                //画矩形
                borderPen = Assist.BorderPen;
                fillBrush = new SolidBrush(BackColor);

                graphics.DrawButtonRect(rect, fillBrush, borderPen, Border.Size,
                    Border.Radius, Border.RadiusCorners, Border.OpenDirection, null);

                //if (ProgressBarRenderer.IsSupported)
                {
                    //ProgressBarRenderer.DrawHorizontalBar(graphics, rect);

                    if (Value > 0)
                    {
                        rect.Inflate(-2, -2);

                        var width = (int)Math.Round(((float)Value / Maximum) * rect.Width);

                        var clip = new Rectangle(rect.X, rect.Y, width, rect.Height);
                        //ProgressBarRenderer.DrawHorizontalChunks(graphics, clip);

                        scaleBrush = Assist.FillBrush;
                        //graphics.FillRectangle(scaleBrush, 2, 2, width, rect.Height);

                        //画进度条
                        graphics.DrawButtonRect(clip, scaleBrush, null, Border.Size,
                            Border.Radius, Border.RadiusCorners, Border.OpenDirection, null);
                    }
                }

                //画文字
                var flags = SkinUtil.GetTextFormatFlags(TextAlign, RightToLeft.Yes == RightToLeft, true);

                graphics.DrawText(CalculateRect(), Text, Assist.TextFont, Assist.TextColor, flags);
            }
            catch (Exception ex) { }
            finally
            {
                fillBrush?.Dispose();
                scaleBrush?.Dispose();
                borderPen?.Dispose();
            }
        }

        private Rectangle CalculateRect()
        {
            var size = TextRenderer.MeasureText(Text, Assist.TextFont);
            var x = (Width - size.Width) / 2;
            var y = (Height - size.Height) / 2;

            switch (TextAlign)
            {
                case HorizontalAlignment.Left:
                    return new Rectangle(1, y, size.Width, size.Height);
                case HorizontalAlignment.Center:
                    return new Rectangle(x, y, size.Width, size.Height);
                case HorizontalAlignment.Right:
                    return new Rectangle(Width - size.Width - 1, y, size.Width, size.Height);
                default:
                    return ClientRectangle;
            }
        }
    }
}