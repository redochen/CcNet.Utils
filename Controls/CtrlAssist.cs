using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CcNet.Utils.Controls
{
    public interface IStateAppearance
    {
        CcAppearances Appearances { get; }
    }

    public interface IBorderSupport
    {
        CcBorder Border { get; }
    }

    /// <summary>
    /// 自定义控件助手类
    /// </summary>
    /// <typeparam name="TCtrl"></typeparam>
    public class CtrlAssist<TCtrl>
        where TCtrl : Control, IStateAppearance, IBorderSupport
    {
        private readonly TCtrl ctrl = null;
        private CcCtrlState state = CcCtrlState.Normal;

        /// <summary>
        /// 获取控件的当前状态
        /// </summary>
        public CcCtrlState State => ctrl.Enabled ? state : CcCtrlState.Disabled;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="setTransparent">设置背景透明</param>
        public CtrlAssist(TCtrl ctrl, bool setTransparent)
        {
            this.ctrl = ctrl;

            //忽略系统描绘的背景
            if (setTransparent)
            {
                this.ctrl.BackColor = Color.FromKnownColor(KnownColor.Transparent);
            }

            //this.ctrl.Font = SystemFonts.DefaultFont;
            this.ctrl.Cursor = Cursors.Hand;

            InitCtrlEvents();
        }

        /// <summary>
        /// 获取控件样式
        /// </summary>
        /// <param name="userPaint">是否启用自动绘制</param>
        /// <param name="supportsTransparent">是否支持背景透明</param>
        /// <returns></returns>
        public static ControlStyles GetStyles(bool userPaint, bool supportsTransparent)
        {
            var styles = ControlStyles.AllPaintingInWmPaint | //忽略擦出的消息，减少闪烁。
                ControlStyles.OptimizedDoubleBuffer |//在缓冲区上绘制，不直接绘制到屏幕上，减少闪烁。
                ControlStyles.ResizeRedraw;  //控件大小发生变化时，重绘。                  

            if (supportsTransparent)
            {
                styles |= ControlStyles.SupportsTransparentBackColor;
            }

            if (userPaint)
            {
                styles |= ControlStyles.UserPaint;  //控件自行绘制，而不使用操作系统的绘制
            }

            return styles;
        }

        /// <summary>
        /// 初始化控件事件
        /// </summary>
        private void InitCtrlEvents()
        {
            ctrl.MouseEnter += (sender, e) => state = CcCtrlState.Hovered;
            ctrl.MouseLeave += (sender, e) => state = CcCtrlState.Normal;
            ctrl.MouseDown += (sender, e) =>
            {
                //鼠标左键且点击次数为1
                if (e.Button == MouseButtons.Left && e.Clicks == 1)
                {
                    state = CcCtrlState.Pressed;
                }
            };
            ctrl.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left && e.Clicks == 1)
                {
                    if (ctrl.ClientRectangle.Contains(e.Location))
                    {
                        //控件区域包含鼠标的位置
                        state = CcCtrlState.Hovered;
                    }
                    else
                    {
                        state = CcCtrlState.Normal;
                    }
                }
            };
            ctrl.GotFocus += (sender, e) => state = CcCtrlState.Focused;
            ctrl.LostFocus += (sender, e) => state = CcCtrlState.Normal;
        }

        /// <summary>
        /// 获取特定状态的外观属性
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private CcAppearance GetAppearance(CcCtrlState state)
        {
            CcAppearance appearance = null;

            switch (state)
            {
                case CcCtrlState.Disabled:
                    appearance = ctrl.Appearances.DisabledAppearance;
                    break;
                case CcCtrlState.Hovered:
                    appearance = ctrl.Appearances.HoveredAppearance;
                    break;
                case CcCtrlState.Pressed:
                    appearance = ctrl.Appearances.PressedAppearance;
                    break;
                case CcCtrlState.Focused:
                    appearance = ctrl.Appearances.FocusedAppearance;
                    break;
                case CcCtrlState.Normal:
                default:
                    break;
            }

            return (appearance?.IsEmpty ?? true) ? ctrl.Appearances.Appearance : appearance;
        }

        #region 获取当前的外观属性
        /// <summary>
        /// 获取文字字体
        /// </summary>
        /// <returns></returns>
        public Font TextFont => GetAppearance(State)?.Font;

        /// <summary>
        /// 获取文字颜色
        /// </summary>
        /// <returns></returns>
        public Color TextColor => GetAppearance(State)?.ForeColor ?? Color.Empty;

        /// <summary>
        /// 获取边框颜色
        /// </summary>
        public Color BorderColor => GetAppearance(State)?.BorderColor ?? Color.Empty;

        /// <summary>
        /// 获取背景渐变起始颜色
        /// </summary>
        public Color BackColorStart => GetAppearance(State)?.BackColorStart ?? Color.Empty;

        /// <summary>
        /// 获取背景渐变结束颜色
        /// </summary>
        public Color BackColorEnd => GetAppearance(State)?.BackColorEnd ?? Color.Empty;

        /// <summary>
        /// 获取背景颜色渐变方向，X轴顺时针开始
        /// </summary>
        public float BackColorAngle => GetAppearance(State)?.BackColorAngle ?? 0.0F;

        /// <summary>
        /// 获取边栏画笔
        /// </summary>
        public Pen BorderPen => ((ctrl.Border?.Size ?? 0) > 0) ? new Pen(BorderColor, ctrl.Border.Size) : null;

        /// <summary>
        /// 获取背景填充画刷
        /// </summary>
        public Brush FillBrush => new LinearGradientBrush(
            ctrl.ClientRectangle, BackColorStart, BackColorEnd, BackColorAngle);
        #endregion
    }
}