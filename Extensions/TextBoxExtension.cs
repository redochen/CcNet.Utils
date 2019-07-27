using System.Windows.Forms;
using CcNet.Utils.Helpers;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// TextBox扩展类
    /// </summary>
    public static class TextBoxExtension
    {
        /// <summary>
        /// 限制输入
        /// </summary>
        /// <param name="ctrl">控件实例</param>
        /// <param name="mode">输入模式</param>
        public static void LimitInput(this TextBox ctrl, TextInputMode mode)
        {
            if (null == ctrl || TextInputMode.All == mode)
            {
                return;
            }

            ctrl.KeyPress += (sender, e) =>
            {
                //控制字符总是允许输入
                if (InputHelper.IsControlChar(e.KeyChar))
                {
                    return;
                }

                var success = InputHelper.CheckInput(mode, e.KeyChar, () => ctrl.Text, false);
                if (!success)
                {
                    e.Handled = true;
                    return;
                }
            };
        }

        /// <summary>
        /// 将光标定位到最后
        /// </summary>
        /// <param name="ctrl"></param>
        public static void ScrollToEnd(this TextBox ctrl)
        {
            if (ctrl != null)
            {
                ctrl.Select(ctrl.Text.Length, 1);
            }
        }
    }
}