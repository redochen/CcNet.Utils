using System;
using System.Drawing;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Color扩展方法
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// 取指定颜色应对的灰度颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetGrayColor(this Color color)
        {
            if (color.IsEmpty)
            {
                return Color.Empty;
            }

            //var gray = (color.R + color.G + color.B) / 3;
            var gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
            gray = GetColorValue(gray);

            return Color.FromArgb(gray, gray, gray);
        }

        /// <summary>
        /// 获取指定颜色的特定透明度的颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alphaRate">透明度百分比（0~100）</param>
        /// <returns></returns>
        public static Color GetAlphaColor(this Color color, float alphaRate)
        {
            if (color.IsEmpty)
            {
                return Color.Empty;
            }

            var alpha = (int)(alphaRate * 255 / 100);
            alpha = GetColorValue(alpha);

            return Color.FromArgb(alpha, color);
        }

        /// <summary>
        /// 获取指定颜色的反转颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetReverseColor(this Color color)
        {
            if (color.IsEmpty)
            {
                return Color.Empty;
            }

            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }

        /// <summary>
        /// 判断两个颜色是否相似
        /// </summary>
        /// <param name="thisColor">当前颜色</param>
        /// <param name="otherColor">要比较的颜色</param>
        /// <param name="thresholdRgbDistance">RGB距离阈值</param>
        /// <param name="thresholdHsbDistance">HSB距离阈值</param>
        /// <param name="factorSaturation">饱和度因子（0~1）</param>
        /// <param name="factorBrightness">亮度因子（0~1）</param>
        /// <returns></returns>
        public static bool? SimilarTo(this Color thisColor, Color otherColor,
            float thresholdRgbDistance = 100.0F, float thresholdHsbDistance = 1.0F,
            float factorSaturation = 1.0F, float factorBrightness = 1.0F)
        {
            if (thisColor.IsEmpty || otherColor.IsEmpty)
            {
                return null;
            }

            var distRGB = thisColor.GetRgbDistance(otherColor);
            var distHSB = thisColor.GetHsbDistance(otherColor,
                factorSaturation: factorSaturation, factorBrightness: factorBrightness);
            return (distRGB <= thresholdRgbDistance && distHSB <= thresholdHsbDistance);
        }

        /// <summary>
        /// 获取两个颜色的RGB色差
        /// </summary>
        /// <param name="thisColor">当前颜色</param>
        /// <param name="otherColor">要比较的颜色</param>
        /// <returns></returns>
        public static float? GetRgbDistance(this Color thisColor, Color otherColor)
        {
            if (thisColor.IsEmpty || otherColor.IsEmpty)
            {
                return null;
            }

            // compute the Euclidean distance between the two colors
            // note, that the alpha-component is not used in this example
            var distRed = Math.Pow(Convert.ToDouble(otherColor.R) - Convert.ToDouble(thisColor.R), 2.0);
            var distGreen = Math.Pow(Convert.ToDouble(otherColor.G) - Convert.ToDouble(thisColor.G), 2.0);
            var distBlue = Math.Pow(Convert.ToDouble(otherColor.B) - Convert.ToDouble(thisColor.B), 2.0);

            return (float)Math.Sqrt(distRed + distGreen + distBlue);
        }

        /// <summary>
        /// 获取两个颜色的HSB(色调-饱和度-亮度)色差
        /// </summary>
        /// <param name="thisColor">当前颜色</param>
        /// <param name="otherColor">要比较的颜色</param>
        /// <param name="factorSaturation">饱和度因数（0~1）</param>
        /// <param name="factorBrightness">亮度因数（0~1）</param>
        /// <returns></returns>
        public static float? GetHsbDistance(this Color thisColor, Color otherColor,
            float factorSaturation = 1.0F, float factorBrightness = 1.0F)
        {
            if (thisColor.IsEmpty || otherColor.IsEmpty)
            {
                return null;
            }

            return Math.Abs(otherColor.GetColorNum(factorSaturation, factorBrightness)
                            - thisColor.GetColorNum(factorSaturation, factorBrightness))
                            + thisColor.GetHueDistance(otherColor);
        }

        /// <summary>
        /// 获取颜色的明亮度(据说Color.GetBrightness不好使)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static float GetBrightnessEx(this Color color)
        {
            if (color.IsEmpty)
            {
                return 0;
            }

            return (color.R * 0.299F + color.G * 0.587F + color.B * 0.114F) / 256F;
        }

        /// <summary>
        /// 获取两个颜色的色调差
        /// </summary>
        /// <param name="thisColor">当前颜色</param>
        /// <param name="otherColor">要比较的颜色</param>
        /// <returns></returns>
        private static float GetHueDistance(this Color thisColor, Color otherColor)
        {
            if (thisColor.IsEmpty || otherColor.IsEmpty)
            {
                return 0;
            }

            var dist = Math.Abs(thisColor.GetHue() - otherColor.GetHue());
            return dist > 180 ? 360 - dist : dist;
        }

        /// <summary>
        /// 获取颜色数值
        /// </summary>
        /// <param name="color"></param>
        /// <param name="factorSaturation">饱和度因子（0~1）</param>
        /// <param name="factorBrightness">亮度因子（0~1）</param>
        /// <returns></returns>
        private static float GetColorNum(this Color color, float factorSaturation, float factorBrightness)
        {
            if (color.IsEmpty)
            {
                return 0;
            }

            return color.GetSaturation() * factorSaturation
                + color.GetBrightnessEx() * factorBrightness;
        }

        /// <summary>
        /// 取颜色值（0 ~255）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int GetColorValue(int value)
        {
            if (value < 0)
            {
                value = 0;
            }

            if (value > 255)
            {
                value = 255;
            }

            return value;
        }
    }
}
