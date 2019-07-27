using System.Drawing;
using System.Windows.Forms;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Form扩展类
    /// </summary>
    public static class FormExtension
    {
        /// <summary>
        /// 设置窗体背景透明（鼠标穿透）
        /// </summary>
        /// <param name="form"></param>
        /// <param name="transparencyKey">透明颜色</param>
        public static void SetTransparent(this Form form, Color transparencyKey)
        {
            if (null == form)
            {
                return;
            }

            form.TransparencyKey = form.BackColor = transparencyKey;
        }

        /// <summary>
        /// 设置窗体背景透明（鼠标不穿透）
        /// </summary>
        /// <param name="form"></param>
        /// <param name="opacity">透明度</param>
        public static void SetTransparent(this Form form, double opacity)
        {
            if (null == form)
            {
                return;
            }

            form.Opacity = opacity;
        }
    }
}