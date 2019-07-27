using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Globalization;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义外观属性类
    /// </summary>
    [TypeConverter(typeof(CcAppearanceTypeConverter))]
    public class CcAppearance
    {
        #region 属性
        /// <summary>
        /// 字体
        /// </summary>
        private Font font = SystemFonts.DefaultFont;
        [Description("获取或设置文字字体"), RefreshProperties(RefreshProperties.All)]
        public Font Font
        {
            get => font;
            set
            {
                if (value != null && !value.Equals(font))
                {
                    IsEmpty = false;
                }

                font = value;
            }
        }

        private Color foreColor = Color.Empty;
        [Description("获取或设置文字颜色"), RefreshProperties(RefreshProperties.All)]
        public Color ForeColor
        {
            get => foreColor;
            set
            {
                if (!value.Equals(foreColor))
                {
                    IsEmpty = false;
                    foreColor = value;
                }
            }
        }

        private Color backColorStart = Color.Empty;
        [Description("获取或设置背景渐变起始颜色"), RefreshProperties(RefreshProperties.All)]
        public Color BackColorStart
        {
            get => backColorStart;
            set
            {
                if (!value.Equals(backColorStart))
                {
                    IsEmpty = false;
                    backColorStart = value;
                }
            }
        }

        private Color backColorEnd = Color.Empty;
        [Description("获取或设置背景渐变结束颜色"), RefreshProperties(RefreshProperties.All)]
        public Color BackColorEnd
        {
            get => backColorEnd;
            set
            {
                if (!value.Equals(backColorEnd))
                {
                    IsEmpty = false;
                    backColorEnd = value;
                }
            }
        }

        /// <summary>
        /// 背景颜色渐变方向，X轴顺时针开始
        /// </summary>
        private float backColorAngle = 0F;
        [Description("获取或设置背景颜色渐变方向，X轴顺时针开始"), RefreshProperties(RefreshProperties.All)]
        public float BackColorAngle
        {
            get => backColorAngle;
            set
            {
                if (backColorAngle != value)
                {
                    IsEmpty = false;
                    backColorAngle = value;
                }
            }
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        private Color borderColor = Color.Empty;
        [Description("获取或设置边框颜色"), RefreshProperties(RefreshProperties.All)]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                if (!value.Equals(borderColor))
                {
                    IsEmpty = false;
                    borderColor = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// 是否为空
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty { get; private set; } = true;

        /// <summary>
        /// 获取背景色
        /// </summary>
        [Browsable(false)]
        public Color BackColor
        {
            get => BackColorStart.IsEmpty ? BackColorEnd : BackColorStart;
        }

        /// <summary>
        /// 获取默认的外观属性
        /// </summary>
        /// <returns></returns>
        public static CcAppearance DefaultAppearance => new CcAppearance
        {
            IsEmpty = false,
            Font = SystemFonts.DefaultFont,
            ForeColor = SystemColors.WindowText,
            BackColorStart = SystemColors.Window,
            BackColorEnd = SystemColors.Window,
            BackColorAngle = 0F,
            BorderColor = SystemColors.ActiveBorder,
        };

        /// <summary>
        /// 获取空的外观属性
        /// </summary>
        public static CcAppearance EmptyAppearance => new CcAppearance { IsEmpty = true };

        /// <summary>
        /// 根据渐变角度获取渐变方向
        /// </summary>
        /// <returns></returns>
        public LinearGradientMode GetGradientMode()
        {
            LinearGradientMode gradientMode;

            if (BackColorAngle >= GradientModeAngles[LinearGradientMode.BackwardDiagonal])
            {
                gradientMode = LinearGradientMode.BackwardDiagonal;
            }
            else if (BackColorAngle >= GradientModeAngles[LinearGradientMode.Horizontal])
            {
                gradientMode = LinearGradientMode.Horizontal;
            }
            else if (BackColorAngle >= GradientModeAngles[LinearGradientMode.ForwardDiagonal])
            {
                gradientMode = LinearGradientMode.ForwardDiagonal;
            }
            else
            {
                gradientMode = LinearGradientMode.Vertical;
            }

            return gradientMode;
        }

        /// <summary>
        /// 根据渐变方向获取渐变角度
        /// </summary>
        /// <param name="gradientMode"></param>
        /// <returns></returns>
        public static float GetAngleByGradientMode(LinearGradientMode gradientMode)
            => GradientModeAngles[gradientMode];

        private static readonly Dictionary<LinearGradientMode, float> GradientModeAngles
            = new Dictionary<LinearGradientMode, float>
            {
                [LinearGradientMode.Horizontal] = 180F,
                [LinearGradientMode.Vertical] = 0F,
                [LinearGradientMode.ForwardDiagonal] = 90F,
                [LinearGradientMode.BackwardDiagonal] = 270F,
            };
    }

    /// <summary>
    /// CcAppearance类型转换器类
    /// </summary>
    public class CcAppearanceTypeConverter : CcTypeConverter
    {
        private const string KeyFont = "Font:";
        private const string KeyForeColor = "ForeColor:";
        private const string KeyBackColorStart = "BackColorStart:";
        private const string KeyBackColorEnd = "BackColorEnd:";
        private const string KeyBackColorAngle = "BackColorAngle:";
        private const string KeyBorderColor = "BorderColor:";

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && !(value is CcAppearance))
            {
                throw WrongTypeException;
            }

            var appearance = value as CcAppearance;

            //转换成字符串
            if (destinationType == typeof(string))
            {
                if (null == appearance)
                {
                    return string.Empty;
                }

                var sb = new StringBuilder();

                if (appearance.Font != null)
                {
                    sb.Append($"{KeyFont}{FontConverter.ConvertToString(context, culture, appearance.Font)}", Separator);
                }

                if (!appearance.ForeColor.IsEmpty)
                {
                    sb.Append($"{KeyForeColor}{ColorConverter.ConvertToString(context, culture, appearance.ForeColor)}", Separator);
                }

                if (!appearance.BackColorStart.IsEmpty)
                {
                    sb.Append($"{KeyBackColorStart}{ColorConverter.ConvertToString(context, culture, appearance.BackColorStart)}", Separator);
                }

                if (!appearance.BackColorEnd.IsEmpty)
                {
                    sb.Append($"{KeyBackColorEnd}{ColorConverter.ConvertToString(context, culture, appearance.BackColorEnd)}", Separator);
                }

                if (appearance.BackColorAngle != 0.0F)
                {
                    sb.Append($"{KeyBackColorAngle}{FloatConverter.ConvertToString(context, culture, appearance.BackColorAngle)}", Separator);
                }

                if (!appearance.BorderColor.IsEmpty)
                {
                    sb.Append($"{KeyBorderColor}{ColorConverter.ConvertToString(context, culture, appearance.BorderColor)}", Separator);
                }

                return sb.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null == value)
            {
                return new CcAppearance();
            }

            //从字符串解析
            if (value is string stringValue)
            {
                if (stringValue.Length <= 0)
                {
                    return new CcAppearance();
                }

                Font font = null;
                Color foreColor = Color.Empty;
                Color backColorStart = Color.Empty;
                Color backColorEnd = Color.Empty;
                float backColorAngle = 0.0F;
                Color borderColor = Color.Empty;

                stringValue.Split(Separator).ForEach(s =>
                {
                    if (s.StartsWith(KeyFont))
                    {
                        font = (Font)FontConverter.ConvertFromString(context, culture, s.Substring(KeyFont.Length));
                    }
                    else if (s.StartsWith(KeyForeColor))
                    {
                        foreColor = (Color)ColorConverter.ConvertFromString(context, culture, s.Substring(KeyForeColor.Length));
                    }
                    else if (s.StartsWith(KeyBackColorStart))
                    {
                        backColorStart = (Color)ColorConverter.ConvertFromString(context, culture, s.Substring(KeyBackColorStart.Length));
                    }
                    else if (s.StartsWith(KeyBackColorEnd))
                    {
                        backColorEnd = (Color)ColorConverter.ConvertFromString(context, culture, s.Substring(KeyBackColorEnd.Length));
                    }
                    else if (s.StartsWith(KeyBackColorAngle))
                    {
                        backColorAngle = (float)FloatConverter.ConvertFromString(context, culture, s.Substring(KeyBackColorAngle.Length));
                    }
                    else if (s.StartsWith(KeyBorderColor))
                    {
                        borderColor = (Color)ColorConverter.ConvertFromString(context, culture, s.Substring(KeyBorderColor.Length));
                    }
                });

                return new CcAppearance
                {
                    Font = font,
                    ForeColor = foreColor,
                    BackColorStart = backColorStart,
                    BackColorEnd = backColorEnd,
                    BackColorAngle = backColorAngle,
                    BorderColor = borderColor,
                };
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}