using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义标签类
    /// </summary>
    [ToolboxItem(true)]
    public class CcLabel : CcTransparent, IStateAppearance, IBorderSupport
    {
        #region 属性
        #region 被隐藏的属性
        [Browsable(false)]
        public new Font Font { get; set; }

        [Browsable(false)]
        public new Color ForeColor { get; set; }

        [Browsable(false)]
        public override Color BackColor { get; set; }

        [Browsable(false)]
        public new Image BackgroundImage { get; set; }

        [Browsable(false)]
        public new ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }
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
        /// 文本对齐方式
        /// </summary>
        private ContentAlignment textAlign = ContentAlignment.TopLeft;
        [Category("CC外观"), Description("获取或设置文本对齐方式"), RefreshProperties(RefreshProperties.All)]
        public ContentAlignment TextAlign
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
        private string text = string.Empty;
        [Category("CC外观"), Description("获取或设置文本内容"), RefreshProperties(RefreshProperties.All)]
        public new string Text
        {
            get => text;
            set
            {
                text = value;
                RecreateHandle();
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public override RightToLeft RightToLeft
        {
            get => base.RightToLeft;
            set
            {
                base.RightToLeft = value;
                RecreateHandle();
            }
        }
        #endregion

        /// <summary>
        /// 控件助手实例
        /// </summary>
        private readonly CtrlAssist<CcLabel> Assist = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CcLabel() : base()
        {
            TabStop = false;

            // TextBox由系统绘制，不能设置 ControlStyles.UserPaint样式
            SetStyle(CtrlAssist<CcLabel>.GetStyles(userPaint: true), true);
            Assist = new CtrlAssist<CcLabel>(this);

            //this.SizeChanged += CcLabel_SizeChanged;
        }

        private void CcLabel_SizeChanged(object sender, EventArgs e)
        {
            var width = Text.GetPixesSize(Assist.TextFont, out int height);
            this.Height = height;
            this.Width = width;
        }

        protected override void OnDraw()
        {
            Pen borderPen = null;
            //Brush textBrush = null;

            try
            {
                borderPen = Assist.BorderPen;

                //画边框
                if (borderPen != null)
                {
                    graphics.DrawButtonRect(ClientRectangle, null, borderPen, Border.Size, Border.Radius);
                }

                //textBrush = new SolidBrush(Assist.TextColor);
                var size = graphics.MeasureString(Text, Assist.TextFont);
                var top = CaculateTop(size.Height);
                var left = CaculateLeft(size.Width);

                //画文字
                graphics.DrawText(ClientRectangle, Text, Assist.TextFont, Assist.TextColor);
                //graphics.DrawString(Text, Assist.TextFont, textBrush, left, top);
            }
            catch (Exception ex) { }
            finally
            {
                //textBrush?.Dispose();
                borderPen?.Dispose();
            }
        }

        private float CaculateTop(float height)
        {
            float top = 0;

            switch (TextAlign)
            {
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    top = (Height - height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    top = Height - height;
                    break;
            }

            return top;
        }

        private float CaculateLeft(float width)
        {
            float left = -1;

            switch (TextAlign)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    if (RightToLeft == RightToLeft.Yes)
                    {
                        left = Width - width;
                    }
                    else
                    {
                        left = -1;
                    }
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    left = (Width - width) / 2;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    if (RightToLeft == RightToLeft.Yes)
                    {
                        left = -1;
                    }
                    else
                    {
                        left = Width - width;
                    }
                    break;
            }

            return left;
        }
    }
}