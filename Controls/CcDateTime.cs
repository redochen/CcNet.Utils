using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义日期时间类
    /// </summary>
    [ToolboxItem(true)]
    public class CcDateTime : DateTimePicker, IStateAppearance, IBorderSupport
    {
        #region 属性
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
        /// 图片与文字的间距
        /// </summary>
        private int dropdownButtonWidth = 18;
        [Category("CC外观"), Description("获取或设置下拉按钮的宽度"), RefreshProperties(RefreshProperties.All)]
        public int DropdownButtonWidth
        {
            get => dropdownButtonWidth;
            set
            {
                dropdownButtonWidth = value;
                Refresh();
            }
        }
        #endregion

        /// <summary>
        /// 控件助手实例
        /// </summary>
        private readonly CtrlAssist<CcDateTime> Assist = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CcDateTime() : base()
        {
            SetStyle(CtrlAssist<CcDateTime>.GetStyles(userPaint: true, supportsTransparent: true), true);
            Assist = new CtrlAssist<CcDateTime>(this, setTransparent: true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = null;
            Pen borderPen = null;
            Brush fillBrush = null;

            try
            {
                graphics = CreateGraphics();
                //Graphics g = e.Graphics;

                //The dropDownRectangle defines position and size of dropdownbutton block, 
                //the width is fixed to 17 and height to 16. The dropdownbutton is aligned to right
                var dropDownRectangle = new Rectangle(
                    ClientRectangle.Width - DropdownButtonWidth - Border.Size/* - Win32.SIZE_SYSTEM*/,
                    Border.Size + Win32.SIZE_SYSTEM, DropdownButtonWidth,
                    ClientRectangle.Height - Border.Size * 2 - Win32.SIZE_SYSTEM);

                borderPen = Assist.BorderPen;
                fillBrush = Assist.FillBrush;

                //填充背景
                graphics.FillRectangle(fillBrush, Border.Size, Border.Size,
                    ClientRectangle.Width - (Border.Size + Win32.SIZE_SYSTEM) * 2 - 1,
                    ClientRectangle.Height - (Border.Size + Win32.SIZE_SYSTEM) * 2 - 1);

                //描绘文字
                //graphics.DrawString(Text, Font, Brushes.Black, 0, 2);
                var textSize = TextRenderer.MeasureText(Text, Assist.TextFont);
                var textRect = new Rectangle((Width - textSize.Width - DropdownButtonWidth) / 2,
                    (Height - textSize.Height) / 2, textSize.Width, textSize.Height);

                var flags = SkinUtil.GetTextFormatFlags(HorizontalAlignment.Center, RightToLeft.Yes == RightToLeft, true);
                TextRenderer.DrawText(graphics, Text, Font, textRect, Assist.TextColor, flags);

                //画下拉按钮
                var visualState = Enabled ? ComboBoxState.Normal : ComboBoxState.Disabled;
                ComboBoxRenderer.DrawDropDownButton(graphics, dropDownRectangle, visualState);

                //画边框
                graphics.DrawRectangle(borderPen, ClientRectangle);
            }
            catch (Exception ex) { }
            finally
            {
                graphics?.Dispose();
                borderPen?.Dispose();
                fillBrush?.Dispose();
            }
        }
    }
}