using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义文本框类
    /// </summary>
    [ToolboxItem(true)]
    public class CcTextBox : TextBox, IStateAppearance, IBorderSupport
    {
        #region 属性
        #region 被隐藏的属性
        [Browsable(false)]
        public new Font Font { get; set; }

        [Browsable(false)]
        public new Color ForeColor { get; set; }

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
        /// 水印文字
        /// </summary>
        private string waterMarkText = string.Empty;
        [Category("CC外观"), Description("获取或设置水印文字"), RefreshProperties(RefreshProperties.All)]
        public string WaterMarkText
        {
            get => waterMarkText;
            set
            {
                waterMarkText = value;
                Refresh();
            }
        }

        /// <summary>
        /// 水印文字的颜色
        /// </summary>
        private Color waterMarkTextColor = Color.Empty;
        [Category("CC外观"), Description("获取或设置水印文字的颜色"), RefreshProperties(RefreshProperties.All)]
        public Color WaterMarkTextColor
        {
            get => waterMarkTextColor;
            set
            {
                waterMarkTextColor = value;
                Refresh();
            }
        }
        #endregion

        /// <summary>
        /// 控件助手实例
        /// </summary>
        private readonly CtrlAssist<CcTextBox> Assist = null;

        /// <summary> 
        ///  默认构造函数
        /// </summary> 
        public CcTextBox() : base()
        {
            // TextBox由系统绘制，不能设置 ControlStyles.UserPaint样式
            SetStyle(CtrlAssist<CcTextBox>.GetStyles(userPaint: false, supportsTransparent: true), true);
            Assist = new CtrlAssist<CcTextBox>(this, setTransparent: true);
        }

        #region 事件
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Border.Radius > 0)
            {
                var rect = new Rectangle(0, 0, Width, Height);
                Region = new Region(rect.GetRoundedRectPath(Border.Radius));
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            //隐藏系统自带的滚动条
            if (ScrollBars != ScrollBars.None)
            {
                //Win32.ShowScrollBar(Handle, Win32.SB_BOTH, false);
            }

            //TextBox是由系统进程绘制，重载OnPaint方法将不起作用
            base.WndProc(ref m);

            if (Win32.WM_PAINT == m.Msg ||
                Win32.WM_NCPAINT == m.Msg ||
                Win32.WM_CTLCOLOREDIT == m.Msg)
            {
                if (BorderStyle.FixedSingle == BorderStyle)
                {
                    using (var graphics = CreateGraphics())
                    {
                        Draw(graphics);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="graphics"></param>
        private void Draw(Graphics graphics)
        {
            Pen borderPen = null;
            //Brush fillBrush = null;

            try
            {
                if (Width <= 0 && Height <= 0)
                {
                    return;
                }

                borderPen = Assist.BorderPen;
                //fillBrush = Assist.FillBrush;

                graphics.DrawButtonRect(ClientRectangle, null, borderPen, Border.Size, Border.Radius);

                if (!Focused && Enabled && !ReadOnly)
                {
                    if (Text != null && Text.Length > 0)
                    {
                        //graphics.DrawText(ClientRectangle, Text, Font, ForeColor,
                        //    _BorderSize, GetTextFormatFlags(TextAlign, RightToLeft));
                    }
                    else if (WaterMarkText != null && WaterMarkText.Length > 0)
                    {
                        if (WaterMarkTextColor.IsEmpty)
                        {
                            WaterMarkTextColor = Assist.TextColor;
                        }

                        var flags = SkinUtil.GetTextFormatFlags(TextAlign, RightToLeft.Yes == RightToLeft, Multiline);

                        //绘制水印文字
                        graphics.DrawText(ClientRectangle, WaterMarkText, Assist.TextFont, WaterMarkTextColor, flags);
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                borderPen?.Dispose();
                //fillBrush?.Dispose();
            }
        }
    }
}
